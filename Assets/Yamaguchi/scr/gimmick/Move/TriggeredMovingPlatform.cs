using UnityEngine;

public class TriggeredMovingPlatform : MonoBehaviour
{
    [Header("動かしたいオブジェクトと位置を合わせてから子にする")]

    [Header("移動経路（順番に移動するポイントたち）")]
    public Transform[] pathPoints;  // 移動するポイントをインスペクターでセット

    [Header("移動速度（単位：単位/秒）")]
    public float moveSpeed = 3f;

    [Header("最初と最後に停止する時間（秒）")]
    public float waitTime = 1f;

    [Header("プレイヤーのレイヤー")]
    public LayerMask playerLayer;

    [Header("プレイヤー検知位置のオフセット")]
    public Vector3 detectionCenterOffset = new Vector3(0f, 0.6f, 0f);

    [Header("プレイヤー検知ボックスサイズ（半径）")]
    public Vector3 detectionHalfExtents = new Vector3(1f, 0.1f, 1f);

    [Header("リスポーン処理")]
    public bool resetOnRespawn = true; // インスペクターでON/OFF切り替え

    // --- 内部変数 ---
    private int currentIndex = 0;    // 現在目標としているポイントのインデックス
    private int direction = 1;       // 移動方向：1=前進、-1=後退
    private bool isWaiting = false;  // 停止中フラグ
    private float waitTimer = 0f;    // 停止時間計測用

    private Vector3 lastPos;         // 前回フレームの位置（プレイヤー同伴移動に使用）

    private bool isActive = false;   // 現在動作中かどうか
    private bool hasTriggered = false; // 今サイクルで一度でもプレイヤーが乗ったか

    void Start()
    {
        if (pathPoints == null || pathPoints.Length == 0)
        {
            Debug.LogError("移動経路のポイント(pathPoints)を設定してください。");
            enabled = false;
            return;
        }

        // 初期位置を最初のポイントにワールド座標で合わせる
        transform.position = pathPoints[0].position;
        lastPos = transform.position;
    }

    void FixedUpdate()
    {
        // 動作していない場合はプレイヤー検知のみ行う
        if (!isActive)
        {
            DetectPlayer();
            return;
        }

        if (isWaiting)
        {
            // 停止時間をカウント
            waitTimer += Time.fixedDeltaTime;
            if (waitTimer >= waitTime)
            {
                // 停止終了
                isWaiting = false;
                waitTimer = 0f;

                // ポイントのインデックスを進める（折り返しも含む）
                currentIndex += direction;

                // 最終点に来たら折り返す
                if (currentIndex >= pathPoints.Length)
                {
                    direction = -1;
                    currentIndex = pathPoints.Length - 2;
                }
                else if (currentIndex < 0)
                {
                    direction = 1;
                    currentIndex = 1;
                }
            }
            else
            {
                // 停止中は移動しない
                return;
            }
        }

        // 次の目標ポイントの位置を取得
        Vector3 targetPos = pathPoints[currentIndex].position;

        // 現在位置から目標に向かうベクトルと距離
        Vector3 moveDir = (targetPos - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPos);

        // 今回フレームで動く距離
        float moveStep = moveSpeed * Time.fixedDeltaTime;

        // 移動量を計算（目標を超えないように調整）
        Vector3 deltaMove;
        if (distanceToTarget <= moveStep)
        {
            // 目標到達
            deltaMove = targetPos - transform.position;

            // 停止状態に入る
            isWaiting = true;

            // ★ スタート地点に戻った場合は停止してリセット
            if (currentIndex == 0 && direction == -1) // 「折り返し後にスタート地点到達」
            {
                isActive = false;
                hasTriggered = false;
                direction = 1; // 次回も正しい方向で始められるようにリセット
                currentIndex = 0;
            }
        }
        else
        {
            deltaMove = moveDir * moveStep;
        }

        // 移動
        transform.position += deltaMove;

        // プレイヤーを同じ距離だけ動かす処理
        MovePlayerWithPlatform(deltaMove);

        // 次フレーム用に位置更新
        lastPos = transform.position;
    }

    /// <summary>
    /// プレイヤーがプラットフォームの上にいたら一緒に移動させる
    /// </summary>
    void MovePlayerWithPlatform(Vector3 deltaMove)
    {
        Vector3 detectionCenter = transform.position + detectionCenterOffset;
        Collider[] hits = Physics.OverlapBox(detectionCenter, detectionHalfExtents, Quaternion.identity, playerLayer);

        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                rb.MovePosition(rb.position + deltaMove);
            }
        }
    }

    /// <summary>
    /// プレイヤーが検知範囲にいるかどうかチェックして起動
    /// </summary>
    void DetectPlayer()
    {
        if (hasTriggered) return; // 今サイクルですでに起動済みなら無視

        Vector3 detectionCenter = transform.position + detectionCenterOffset;
        Collider[] hits = Physics.OverlapBox(detectionCenter, detectionHalfExtents, Quaternion.identity, playerLayer);
        if (hits.Length > 0)
        {
            isActive = true;
            hasTriggered = true;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 detectionCenter = transform.position + detectionCenterOffset;
        Gizmos.DrawWireCube(detectionCenter, detectionHalfExtents * 2);

        if (pathPoints != null && pathPoints.Length > 1)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                if (pathPoints[i] != null && pathPoints[i + 1] != null)
                {
                    Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
                }
            }
        }
    }
#endif

   

    public void ResetPlatform()
    {
        if (!resetOnRespawn) return;
        if (pathPoints == null || pathPoints.Length == 0) return;

        transform.position = pathPoints[0].position;
        lastPos = transform.position;

        currentIndex = 0;
        direction = 1;
        isActive = false;
        hasTriggered = false;
        isWaiting = false;
        waitTimer = 0f;
    }

}
