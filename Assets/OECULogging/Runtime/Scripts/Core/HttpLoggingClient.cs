using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace com.naosv.OECULogging.Core
{
    /// <summary>
    /// キュー→一時ファイル（プレーンテキスト行）→定期/強制で
    /// { app, logs } 形式にまとめてPOST。
    /// logsは「そのまま保存可能なテキスト塊」（改行含む）。
    /// </summary>
    internal class HttpLoggingClient
    {
        #region 変数

        // ==== メインのログ処理用 ====
        private readonly string _productName;
        private readonly string _tmpFullPath;
        private readonly TimeSpan _flushInterval;

        private readonly ConcurrentQueue<string> _queue = new();
        private readonly AutoResetEvent _signal = new(false);

        private static readonly HttpClient _httpClient =
            new HttpClient() { Timeout = TimeSpan.FromSeconds(10) };

        private Thread _worker;
        private CancellationToken _tk;
        private volatile string _endpoint;
        private volatile bool _forceFlush;

        // バッチ閾値（必要に応じて調整）
        private const int MaxBatchLinesDefault = 2000;           // 1バッチ最大行数
        private const int MaxLogsBytesDefault = 512 * 1024;     // logs文字列の最大バイト数(UTF-8)

        private readonly int _maxBatchLines;
        private readonly int _maxLogsBytes;

        [Serializable]
        private struct BatchPayload
        {
            public string app;
            public string logs; // そのまま書き込めるテキスト塊
        }

        // ==== Discord Webhook 用 ====
        private volatile string _webhookUrl;
        private volatile bool _webhookEnabled;
        private readonly ConcurrentQueue<string> _whQueue = new();
        private readonly AutoResetEvent _whSignal = new(false);
        private Thread _whWorker;
        private static readonly TimeSpan WebhookInterval = TimeSpan.FromSeconds(1);
        private const int WebhookMaxChars = 1000; // 1回の文章の上限

        // 送信対象タイプ（大文字小文字を区別せず比較）
        private readonly HashSet<string> _whTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ERROR", "EXCEPTION"
        };

        [Serializable] private struct DiscordPayload { public string content; }  // Webhook JSON

        #endregion

        #region コンストラクタ

        internal HttpLoggingClient(string productName, string persistentDataPath, TimeSpan flushInterval,
                                   int maxBatchLines = MaxBatchLinesDefault, int maxLogsBytes = MaxLogsBytesDefault)
        {
            _productName = productName;
            _tmpFullPath = Path.Combine(persistentDataPath, "oeculogging_tmp.txt");
            _flushInterval = flushInterval;
            _maxBatchLines = Math.Max(1, maxBatchLines);
            _maxLogsBytes = Math.Max(64 * 1024, maxLogsBytes); // 最低64KB
        }

        #endregion

        #region 内部API

        internal void Start(CancellationToken tk)
        {
            _tk = tk;
            _worker = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = "OECU-LogWorker"
            };
            _worker.Start();

            _whWorker = new Thread(WebhookLoop)
            {
                IsBackground = true,
                Name = "OECU-WebhookWorker"
            };
            _whWorker.Start();
        }

        internal void StopAndFlush(TimeSpan joinTimeout)
        {
            try
            {
                _forceFlush = true;
                _signal.Set(); // 最終フラッシュを即起動
                _whSignal.Set();
                _worker?.Join((int)joinTimeout.TotalMilliseconds);
                _whWorker?.Join((int)joinTimeout.TotalMilliseconds);
            }
            catch { /* ignore */ }
        }

        /// <summary>
        /// メインスレッドからは「1行分」の文字列に整形し、キューへ積むだけ。
        /// 改行は行内に含めず、メッセージ内の改行は "\\n" にエスケープして1行化。
        /// </summary>
        internal void Enqueue(string message, string logType)
        {
            string time = DateTime.Now.ToString("o");
            string level = $"[{logType}]";
            string msg = message.Trim().Replace("\r", "");

            // 1エントリ = 「ISO8601 [LVL] メッセージ」
            string line = $"{time} {level} {msg}";
            _queue.Enqueue(line);

            // 起床はする（ただし送信はinterval/強制時のみ）
            _signal.Set();

            if (_webhookEnabled &&
                !string.Equals(logType, "OECULogging", StringComparison.OrdinalIgnoreCase) &&
                _whTypes.Contains(logType))
            {
                _whQueue.Enqueue(line);
            }
        }

        public void SetEndPoint(string url)
        {
            _endpoint = url;
            _forceFlush = true; // 設定直後に送信試行
            _signal.Set();
        }

        internal void EnableWebhook(string url)
        {
            _webhookUrl = url;
            _webhookEnabled = !string.IsNullOrWhiteSpace(url);
            _whSignal.Set();
        }

        internal void DisableWebhook()
        {
            TrySendWebhookBatch_FireAndForget(); // 最終送信
            _webhookEnabled = false;
            _webhookUrl = null;
            while (_whQueue.TryDequeue(out _)) { }
        }

        internal void AddWebhookType(string type)
        {
            if (!string.IsNullOrWhiteSpace(type)) _whTypes.Add(type);
        }

        internal void RemoveWebhookType(string type)
        {
            if (!string.IsNullOrWhiteSpace(type)) _whTypes.Remove(type);
        }

        #endregion

        #region ログ処理

        private void WorkerLoop()
        {
            DateTime lastFlush = DateTime.UtcNow;

            while (true)
            {
                int waitMs = (int)Math.Max(0, (_flushInterval - (DateTime.UtcNow - lastFlush)).TotalMilliseconds);
                if (waitMs == 0) waitMs = (int)_flushInterval.TotalMilliseconds;

                int signaled = WaitHandle.WaitAny(new WaitHandle[] { _signal, _tk.WaitHandle }, waitMs);

                // まずは必ずファイルへ吐く（ここは高速・例外握り潰しでゲーム非干渉）
                DrainQueueToFile();

                if (_tk.IsCancellationRequested || signaled == 1)
                {
                    FlushFileToServer(); // 最終フラッシュ
                    return;
                }

                bool dueByInterval = (DateTime.UtcNow - lastFlush) >= _flushInterval;
                if (dueByInterval || _forceFlush)
                {
                    FlushFileToServer();
                    lastFlush = DateTime.UtcNow;
                    _forceFlush = false;
                }
            }
        }

        /// <summary>
        /// キュー→一時ファイル（1行=1エントリ）。AppendAllLines は自動で改行付与。
        /// </summary>
        private void DrainQueueToFile()
        {
            if (_queue.IsEmpty) return;

            var batch = new List<string>(256);
            while (_queue.TryDequeue(out var line))
            {
                batch.Add(line);
                if (batch.Count >= 256) break; // 大量投入でも分割しつつ追記
            }
            if (batch.Count == 0) return;

            try
            {
                File.AppendAllLines(_tmpFullPath, batch);
            }
            catch
            {
                // 失敗してもゲームは止めない（次回へ）
            }
        }

        /// <summary>
        /// 一時ファイルを読み、複数行をまとめて { app, logs } としてPOST。
        /// 成功した塊は破棄、失敗は残す（再送保証）。
        /// </summary>
        private void FlushFileToServer()
        {
            if (string.IsNullOrEmpty(_endpoint)) return;

            string[] lines;
            try
            {
                if (!File.Exists(_tmpFullPath)) return;
                lines = File.ReadAllLines(_tmpFullPath);
                if (lines.Length == 0)
                {
                    File.Delete(_tmpFullPath);
                    return;
                }
            }
            catch { return; }

            var remain = new List<string>(lines.Length);

            // バイトサイズ/行数でチャンク分割して送信
            int i = 0;
            while (i < lines.Length)
            {
                var sb = new StringBuilder(capacity: 4096);
                int usedBytes = 0;
                int usedLines = 0;

                while (i < lines.Length && usedLines < _maxBatchLines)
                {
                    string add = lines[i];
                    // 行の後ろに改行をつけて塊にする
                    string addWithNl = add + "\n";
                    int addBytes = Encoding.UTF8.GetByteCount(addWithNl);

                    if (usedLines > 0 && usedBytes + addBytes > _maxLogsBytes)
                        break; // 既に何か詰めていて閾値超過しそうなら送信へ

                    // 1行目が巨大すぎる場合でも最低1行は送る
                    if (usedLines == 0 && addBytes > _maxLogsBytes)
                    {
                        // 可能ならそのまま送る（失敗したら残す）
                        sb.Append(addWithNl);
                        usedBytes += addBytes;
                        usedLines++;
                        i++;
                        break;
                    }

                    sb.Append(addWithNl);
                    usedBytes += addBytes;
                    usedLines++;
                    i++;
                }

                string block = sb.ToString();
                if (!TryPostBlock(block).GetAwaiter().GetResult())
                {
                    // 失敗したブロックは残す
                    // （行単位管理なので、そのまま書き戻せば次回同じ順序で再送される）
                    var failedLines = block.Split('\n');
                    // Split末尾の空行対策
                    for (int k = 0; k < failedLines.Length; k++)
                        if (!string.IsNullOrEmpty(failedLines[k]))
                            remain.Add(failedLines[k]);
                }
            }

            try
            {
                if (remain.Count == 0)
                {
                    File.Delete(_tmpFullPath);
                }
                else
                {
                    File.WriteAllLines(_tmpFullPath, remain);
                }
            }
            catch
            {
                // 書けなくても次回で再試行
            }
        }

        /// <summary>
        /// { app, logs } のJSONを組んでPOST（logsは整形済みテキスト塊）
        /// </summary>
        private async Task<bool> TryPostBlock(string logsBlock)
        {
            try
            {
                var payload = new BatchPayload { app = _productName, logs = logsBlock };
                string json = JsonUtility.ToJson(payload); // 改行等はJSONエスケープされる
                var res = await _httpClient.PostAsync(
                    _endpoint, new StringContent(json, Encoding.UTF8, "application/json")
                ).ConfigureAwait(false);
                return res.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Webhook処理

        private void WebhookLoop()
        {
            DateTime last = DateTime.UtcNow;

            while (true)
            {
                int waitMs = (int)Math.Max(0, (WebhookInterval - (DateTime.UtcNow - last)).TotalMilliseconds);
                if (waitMs == 0) waitMs = (int)WebhookInterval.TotalMilliseconds;

                int signaled = WaitHandle.WaitAny(new WaitHandle[] { _whSignal, _tk.WaitHandle }, waitMs);
                if (_tk.IsCancellationRequested || signaled == 1) {
                    TrySendWebhookBatch_FireAndForget(); // 最終送信
                    return;
                }

                bool due = (DateTime.UtcNow - last) >= WebhookInterval;
                if (due)
                {
                    last = DateTime.UtcNow;
                    TrySendWebhookBatch_FireAndForget();
                }
            }
        }

        private void TrySendWebhookBatch_FireAndForget()
        {
            if (!_webhookEnabled || string.IsNullOrWhiteSpace(_webhookUrl) || _whQueue.IsEmpty) return;

            // 1000文字上限まで詰める（超えたら "..." を付ける。これは上限に含めない）
            var sb = new StringBuilder(capacity: 1024);
            int used = 0;
            bool truncated = false;

            while (_whQueue.TryDequeue(out var line))
            {
                string add = (sb.Length == 0) ? line : ("\n" + line);
                if (used + add.Length > WebhookMaxChars)
                {
                    // まだ何も入っていないなら先頭を切り詰め
                    if (used == 0)
                    {
                        sb.Append(line.Substring(0, Math.Min(line.Length, WebhookMaxChars)));
                    }
                    truncated = true;
                    break;
                }
                sb.Append(add);
                used += add.Length;
            }

            if (sb.Length == 0) return;
            if (truncated || !_whQueue.IsEmpty) sb.Append("..."); // "..." は上限に含めない

            string content = sb.ToString();
            string url = _webhookUrl; // volatile 読み出し

            // 非同期・投げっぱなし（結果は ContinueWith で拾えたらログ）
            _ = PostDiscordAsync(url, content).ContinueWith(t =>
            {
                // ブロックしない＆失敗しても再送しない
                if (t.Exception != null || !t.Result)
                {
                    Enqueue("Discord Webhook post failed.", "OECULogging");
                }
                else
                {
                    Enqueue("Discord Webhook post succeeded.", "OECULogging");
                }
            }, TaskScheduler.Default);
        }

        private async Task<bool> PostDiscordAsync(string url, string content)
        {
            try
            {
                var payload = new DiscordPayload { content = content };
                string json = JsonUtility.ToJson(payload);
                var res = await _httpClient
                    .PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"))
                    .ConfigureAwait(false);
                return res.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
