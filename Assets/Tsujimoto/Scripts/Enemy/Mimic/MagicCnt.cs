using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCnt : MonoBehaviour
{
    [Header("魔法のスピード")]
    [HideInInspector] public float speed = 10f;

    private Vector3 direction; // 方向ベクトル
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed; // 方向 × 速度

        StartCoroutine(DestroyBall());
    }

    // ターゲット位置を渡して、方向を設定する
    public void Init(Vector3 targetPosition)
    {
        direction = (targetPosition - transform.position).normalized;

        // ターゲットの方向を向く
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    // スピードを設定
    public void SetSpeed(float x)
    {
        speed = x;
    }

    //指定の秒数後に削除
    IEnumerator DestroyBall()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2") || other.gameObject.layer == LayerMask.NameToLayer("BringObj"))
        {
            Destroy(gameObject);
        }

        if (other.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }
}