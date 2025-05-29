using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    Rigidbody rb;
    GameObject player1;
    [Header("弾の速度")]
    public float speed;

    [Header("ファイヤーのエフェクト")]
    public GameObject hitEffectPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player1 = GameObject.Find("Player1");
        //放物線上に飛ばす
        Vector3 direction = (player1.transform.forward + player1.transform.up).normalized;
        rb.AddForce(direction * speed, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        //床に触れたら
        if (other.CompareTag("Floor") || other.CompareTag("Wall"))
        {
            //エフェクトを展開
            Quaternion rotation = Quaternion.Euler(90, 0, 0); //角度を調整
            Instantiate(hitEffectPrefab, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z),
             rotation);
            Destroy(gameObject); //弾を削除
        }
    }
}
