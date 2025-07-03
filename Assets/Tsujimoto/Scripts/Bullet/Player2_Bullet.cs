using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player2_Bullet : MonoBehaviour
{
    Rigidbody rb;
    GameObject player2;
    [Header("弾の速度")]
    public float speed;

    [Header("ファイヤーのエフェクト")]
    public GameObject hitEffectPrefab;

    SoundManager soundManager; //SoundManagerのインスタンス
    SoundsList soundsList; //SoundsListのインスタンス
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player2 = GameObject.Find("Player2");
        //放物線上に飛ばす
        // Vector3 direction = (player2.transform.forward + player2.transform.up).normalized;
        Vector3 direction = (player2.transform.forward).normalized;
        rb.AddForce(direction * speed, ForceMode.Impulse);

        soundManager = GameObject.FindObjectOfType<SoundManager>();
        soundsList = GameObject.FindObjectOfType<SoundsList>();
    }

    void OnTriggerEnter(Collider other)
    {
        //床に落ちたら
        if (other.CompareTag("Floor") || other.CompareTag("Wall"))
        {
            //エフェクトを展開
            Quaternion rotation = Quaternion.Euler(90, 0, 0); //角度を調整
            Instantiate(hitEffectPrefab, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z),
             rotation);

            Destroy(gameObject); //弾を削除
        }
    }
}
