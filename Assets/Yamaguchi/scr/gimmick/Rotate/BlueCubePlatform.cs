using UnityEngine;

public class BlueCubePlatform : MonoBehaviour
{
    // 前フレームの位置と回転を保存しておく
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private void Start()
    {
        // 初期化：スタート時の位置と回転を保存
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        // プレイヤー1 または プレイヤー2 だけ処理する
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            // 接触したオブジェクトに Rigidbody があるか確認
            Rigidbody playerRb = collision.rigidbody;

            if (playerRb != null)
            {
                // 【1】足場の回転の差分を計算する
                // 今フレームの回転 × 1フレーム前の回転の逆 → 差分だけが取れる
                Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);

                // 【2】プレイヤーの位置を足場ローカル基準で取得
                Vector3 localPos = playerRb.position - transform.position;

                // 【3】回転差分をプレイヤーのローカル位置に適用
                Vector3 rotatedPos = deltaRotation * localPos;

                // 【4】足場基準に戻して新しいワールド位置を決定
                Vector3 newPlayerPos = transform.position + rotatedPos;

                // 【5】新しい位置と現在位置の差分を求める
                Vector3 movement = newPlayerPos - playerRb.position;

                // 【6】Rigidbody.MovePosition で物理的に安全に移動
                playerRb.MovePosition(playerRb.position + movement);

                // 次のフレーム用に現在の位置と回転を保存しておく
                lastPosition = transform.position;
                lastRotation = transform.rotation;
            }
        }
    }
}
