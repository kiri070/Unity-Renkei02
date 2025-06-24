using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // === 公開パラメータ（インスペクターで調整可能） ===

    [Header("往復移動の振れ幅（XYZ軸それぞれ）")]
    // 各軸でどのくらい動くか（例：X=3ならX方向に±3動く）
    public Vector3 moveAmplitude = Vector3.zero;

    [Header("往復移動の速さ（周期：XYZ別）")]
    // 各軸ごとの動きの速さ（1秒間に何回往復するか）
    public Vector3 moveFrequency = Vector3.zero;

    [Header("プレイヤーのレイヤー")]
    // プレイヤーが乗ったことを検出するためのレイヤー（インスペクターでレイヤー指定）
    public LayerMask playerLayer;

    [Header("プレイヤー検知位置のオフセット")]
    // 床の中心からどの位置に検知用ボックスをずらすか
    public Vector3 detectionCenterOffset = new Vector3(0f, 0.6f, 0f);

    [Header("プレイヤー検知ボックスサイズ（半径）")]
    // 検知する範囲の大きさ（中心からの距離）
    public Vector3 detectionHalfExtents = new Vector3(1f, 0.1f, 1f);

    // === 内部変数 ===

    private Vector3 startPos;   // 開始位置（初期位置）
    private Vector3 lastPos;    // 前回の位置（プレイヤーの移動量計算に使う）

    void Start()
    {
        // 開始時の位置を記録（中心位置として使う）
        startPos = transform.position;
        lastPos = startPos;
    }

    void FixedUpdate()
    {
        // 時間経過に応じてオフセットを計算（Sin波で往復）
        float t = Time.time;

        Vector3 offset = Vector3.zero;

        // X軸に動きがある場合のみ計算
        if (moveAmplitude.x != 0f)
            offset.x = Mathf.Sin(t * moveFrequency.x) * moveAmplitude.x;

        // Y軸に動きがある場合のみ計算
        if (moveAmplitude.y != 0f)
            offset.y = Mathf.Sin(t * moveFrequency.y) * moveAmplitude.y;

        // Z軸に動きがある場合のみ計算
        if (moveAmplitude.z != 0f)
            offset.z = Mathf.Sin(t * moveFrequency.z) * moveAmplitude.z;

        // 新しい位置を決定（中心 + オフセット）
        Vector3 newPos = startPos + offset;

        // どれだけ動いたか（プレイヤーに同じだけ移動させる）
        Vector3 deltaMove = newPos - lastPos;

        // 床を新しい位置に移動
        transform.position = newPos;

        // プレイヤーが乗っていたら一緒に動かす
        MovePlayerWithPlatform(deltaMove);

        // 次回用に現在位置を保存
        lastPos = newPos;
    }

    // プレイヤーを検知して、一緒に動かす
    void MovePlayerWithPlatform(Vector3 deltaMove)
    {
        // プレイヤー検知用の中心位置を計算
        Vector3 center = transform.position + detectionCenterOffset;

        // 指定したレイヤー上のプレイヤーを検知（OverlapBox）
        Collider[] hits = Physics.OverlapBox(center, detectionHalfExtents, Quaternion.identity, playerLayer);

        foreach (Collider hit in hits)
        {
            // Rigidbody を持っているか確認
            Rigidbody rb = hit.attachedRigidbody;

            // プレイヤーを MovePosition で同じ分だけ動かす
            if (rb != null && !rb.isKinematic)
            {
                rb.MovePosition(rb.position + deltaMove);
            }
        }
    }

    // エディタ上で検知範囲の可視化（選択時）
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        // 検知範囲の表示（薄青いボックス）
        Vector3 center = transform.position + detectionCenterOffset;
        Gizmos.DrawWireCube(center, detectionHalfExtents * 2);
    }
#endif
}
