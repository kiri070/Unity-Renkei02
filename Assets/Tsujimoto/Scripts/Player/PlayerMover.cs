using UnityEngine;
using UnityEngine.Video;

public class PlayerMover : MonoBehaviour
{
    Rigidbody rb;
    Vector3 move; //移動するためのベクトル
    PlayerCnt playerCnt;

    [HideInInspector]
    public float jumpForce; //ジャンプ力
    [HideInInspector]
    public bool jumping; //ジャンプをするか
    [HideInInspector]
    public bool canJump; //ジャンプが可能か
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCnt = GameObject.FindObjectOfType<PlayerCnt>();
    }

    //移動ベクトルをこのキャラのベクトルに代入
    public void Assignment(Vector3 input)
    {
        move = input;
    }

    void Update()
    {
        OffScreen();
    }

    void FixedUpdate()
    {
        Move();
    }

    //移動処理
    void Move()
    {
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        // 向き変更（移動中のみ）
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }

        //ジャンプ
        if (jumping)
        {
            rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
            jumping = false;
            canJump = false;
        }        
    }

    //スライディング開始:大きさを小さく
    public void StartSliding()
    {
        Vector3 player1Scale = transform.localScale;
        player1Scale.y = 0.5f;
        transform.localScale = player1Scale;
    }
    //スライディング終了:大きさを戻す
    public void EndSliding()
    {
        Vector3 player1Scale = transform.localScale;
        player1Scale.y = 1f;
        transform.localScale = player1Scale;
    }

    //画面外検知処理
    void OffScreen()
    {
        //このオブジェクトをカメラの画面上での位置に変換
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        //画面外に出た時
        if (viewPos.x < 0 || viewPos.x > 1 ||
           viewPos.y < 0 || viewPos.y > 1 ||
           viewPos.z < 0)
        {
            //ジャンプしていないとき
            if (canJump)
                GameManager.ToGameOverState();
        }
    }

    //CollisionEnter
    void OnCollisionEnter(Collision other)
    {
        //地面に触れたら
        if (other.gameObject.CompareTag("Floor"))
        {
            canJump = true;
        }
    }

    //TriggerEnter
    void OnTriggerEnter(Collider other)
    {
        //GameOverWallに触れたら
        if (other.CompareTag("GameOverWall"))
        {
            GameManager.ToGameOverState();
            Debug.Log("ゲームオーバー");
        }
    }

    public static void GameClear()
    {
        GameManager.ToClearState();
        Debug.Log("クリア");
    }
}
