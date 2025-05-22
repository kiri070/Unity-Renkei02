using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤーの速度")]
    [SerializeField]
    [Range(0, 30)]
    private float moveSpeed; //プレイヤーの移動速度

    Rigidbody rb; //Rigidbody
    void Start()
    {
        rb = GetComponent<Rigidbody>(); //Rigidbodyを取得
    }

    void Update()
    {
        Move();
    }

    void FixedUpdate()
    {
        
    }

    //移動処理
    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //入力をベクトルに変換
        Vector3 input = new Vector3(x, 0f, z) * moveSpeed;

        //向きに合わせて移動方向を変換
        Vector3 move = transform.TransformDirection(input);

        //移動
        rb.velocity = new Vector3(move.x, 0f, move.z);
    }
}
