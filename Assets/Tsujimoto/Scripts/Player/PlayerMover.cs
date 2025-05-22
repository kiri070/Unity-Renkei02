using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    Rigidbody rb;
    Vector3 move; //移動するためのベクトル

    [HideInInspector]
    public float jumpForce; //ジャンプ力
    [HideInInspector]
    public bool jumping; //ジャンプをするか
    [HideInInspector]
    public bool canJump; //ジャンプが可能か
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //移動ベクトルをこのキャラのベクトルに代入
    public void Assignment(Vector3 input)
    {
        move = input;
    }

    void FixedUpdate()
    {
        Move();
    }

    //移動処理
    void Move()
    {
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        //ジャンプ
        if (jumping)
        {
            rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
            jumping = false;
            canJump = false;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        //地面に触れたら
        if (other.gameObject.CompareTag("Floor"))
        {
            canJump = true;
        }
    }
}
