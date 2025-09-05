using System.Threading.Tasks;
using UnityEngine;
using System.Net.Http;
using System;
using System.Threading;
using System.IO;

namespace com.naosv.OECULogging.Core
{
    internal static class FetchServerAddress
    {
        private static readonly string ENDPOINT = "http://dench.mklab.osakac.ac.jp/api/endpoint_library.php?name=oeculogging";
        private static string _host = null;
        private static int _port = 0;

        [Serializable]
        private struct ServerInfo
        {
            public string host;
            public int port;
        }

        private static bool TryLoadLocalOverride()
        {
            var path = Path.Combine(Application.streamingAssetsPath,
                                    "oeculogging_endpoint_override.json");
            if (File.Exists(path) && TryParseJson(path, out _host, out _port)) return true;

            return false;
        }
        
        private static bool TryParseJson(string file, out string host, out int port)
        {
            var json = File.ReadAllText(file);
            var d    = JsonUtility.FromJson<ServerInfo>(json);
            host = d.host;  port = d.port;
            return !string.IsNullOrEmpty(host);
        }

        private static async Task FetchOnceAsync()
        {
            if (TryLoadLocalOverride())
            {
                Debug.Log("[OECU] Using local endpoint override.");
                return;
            }

            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var json = await client.GetStringAsync(ENDPOINT);

            var result = JsonUtility.FromJson<ServerInfo>(json);
            _host = result.host;
            _port = result.port;
        }

        internal static async Task FetchWithRetryAsync(CancellationToken tk)
        {
            var delay = 1;
            while (!tk.IsCancellationRequested)
            {
                try
                {
                    await FetchOnceAsync();
                    if (!string.IsNullOrEmpty(_host)) return;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[OECU] Fetch failed: {e.Message}  retry in {delay}s");
                }
                await Task.Delay(TimeSpan.FromSeconds(delay), tk);
                delay = Math.Min(delay * 2, 60);
            }
        }

        internal static string Host => _host;
        internal static int Port => _port;

    }
}