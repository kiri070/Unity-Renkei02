using UnityEngine;

public class NewBlueCubePlatform : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // プレイヤー1 または プレイヤー2 の場合
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            // プレイヤーを青キューブの子にする
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // プレイヤーが離れたら親子関係解除
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            collision.transform.SetParent(null);
        }
    }
}
