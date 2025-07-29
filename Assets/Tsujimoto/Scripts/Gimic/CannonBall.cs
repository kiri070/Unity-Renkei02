using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [Header("爆発設定")]
    [Tooltip("爆発力")] public float explosionForce = 10f;
    [Tooltip("爆発範囲(半径)")][SerializeField] public float radius = 3f;
    [Tooltip("上方向に加える力")][SerializeField] public float upForce = 0f;

    [Header("エフェクト")]
    [Tooltip("弾が当たった時")][SerializeField] GameObject hitEffect;
    [Header("自動削除")]

    [Tooltip("弾が自動的に消えるまでの秒数")][SerializeField] float lifeTime = 7f;


    void Start()
    {
        StartCoroutine(DestroyBall());
    }
    void OnTriggerEnter(Collider other)
    {
        //プレイヤーに当たった場合
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            //爆発処理
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.AddExplosionForce(explosionForce, transform.position, radius, upForce, ForceMode.Impulse);

            //エフェクト
            Instantiate(hitEffect, transform.position, hitEffect.transform.rotation);

            Destroy(gameObject);
        }
        //地面に当たった場合
        if (other.CompareTag("Floor"))
        {
            //エフェクト
            Instantiate(hitEffect, transform.position, hitEffect.transform.rotation);

            Destroy(gameObject);
        }
    }

    //指定の秒数後に削除
    IEnumerator DestroyBall()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
