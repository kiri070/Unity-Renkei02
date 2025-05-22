using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤーの速度")]
    [SerializeField]
    [Range(0, 30)]
    private float moveSpeed; //プレイヤーの移動速度

    [SerializeField]
    private float jumpForce; //プレイヤーのジャンプ力

    Rigidbody rb; //Rigidbody
    Vector3 input; //向きに合わせて移動するための変数
    private bool jumping; //ジャンプをするか
    private bool canJump; //ジャンプができるかどうか
    void Start()
    {
        rb = GetComponent<Rigidbody>(); //Rigidbodyを取得
    }

    void Update()
    {
        if (!Cursor.visible)
        {
            Controlle();
        }
    }

    void FixedUpdate()
    {
        if (!Cursor.visible)
        {
            Move();
        }
    }

    //操作
    void Controlle()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //入力をベクトルに変換
        input = new Vector3(x, 0f, z) * moveSpeed;

        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            jumping = true;
            canJump = false;
        }
    }

    //移動処理
    void Move()
    {
        //向きに合わせて移動方向を変換
        Vector3 move = transform.TransformDirection(input);
        //移動
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        //ジャンプ
        if (jumping)
        {
            rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
            jumping = false;
        }
    }

    //当たり判定
    void OnCollisionEnter(Collision other)
    {
        //床に触れた時
        if (other.gameObject.CompareTag("Floor"))
        {
            canJump = true; //ジャンプを可能にする
        }
    }
}
