using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("爆発力")][SerializeField] float explosionForce = 10f;
    [Header("爆発範囲(半径)")][SerializeField] float radius = 3f;
    [Header("上方向に加える力")][SerializeField] float upForce = 0f;
    Rigidbody rb;

    [Header("爆発のエフェクト")] public GameObject explosionEffect;

    SoundManager soundManager; //SoundManagerのインスタンス
    SoundsList soundsList; //SoundListのインスタンス

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
    }
    void OnTriggerEnter(Collider other)
    {
        //プレイヤーが触れた時
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            //一定範囲のオブジェクトを取得
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hit in colliders)
            {
                Rigidbody hitRb = hit.GetComponent<Rigidbody>();
                if (hitRb != null && hitRb != rb)
                {
                    //爆発
                    hitRb.AddExplosionForce(explosionForce, transform.position, radius, upForce, ForceMode.Impulse);
                    //エフェクト再生
                    if (explosionEffect != null) Instantiate(explosionEffect, transform.position, Quaternion.identity);

                    //SE再生
                    soundManager.OnPlaySE(soundsList.explosionSE);
                }
            }

            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
