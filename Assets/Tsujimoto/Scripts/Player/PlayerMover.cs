using Unity.VisualScripting;
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

    [Header("各カメラを格納")]
    public Camera gameCamera;

    [Header("各プレイヤーのオブジェクトを格納")]
    public GameObject player;

    [Header("マテリアル")]
    public Material defaultMaterial, hideMaterial;

    Renderer renderer;

    //氷関連
    bool onIce; //氷の上にいるか
    Vector3 slideVelocity; //滑り中の速度
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCnt = GameObject.FindObjectOfType<PlayerCnt>();
        renderer = GetComponent<Renderer>();
    }

    //移動ベクトルをこのキャラのベクトルに代入
    public void Assignment(Vector3 input)
    {
        move = input;
    }

    void Update()
    {
        OffScreen();
        WallChecker();
    }

    void FixedUpdate()
    {
        Move();
    }

    //移動処理
    void Move()
    {
        //氷の上の場合
        if (onIce)
        {
            //入力がある場合
            if (move.magnitude > 0.1f)
            {
                slideVelocity = move;
            }
            //入力がない場合
            else
            {
                slideVelocity *= 0.96f; //少し滑らせる
            }

            rb.velocity = new Vector3(slideVelocity.x, rb.velocity.y, slideVelocity.z);
        }
        //通常の移動
        else
        {
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        }

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
        Vector3 viewPos = gameCamera.WorldToViewportPoint(transform.position);
        // Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

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

    //壁などに隠れている時
    void WallChecker()
    {
        Vector3 camPos = gameCamera.transform.position;
        Vector3 targetPos = player.transform.position;
        Vector3 direction = targetPos - camPos;

        //カメラからプレイヤーまでの直線上にRayを飛ばす
        if (Physics.Raycast(camPos, direction.normalized, out RaycastHit hit, direction.magnitude))
        {
            //隠れていたら
            if (hit.collider.gameObject != player)
            {
                if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor"))
                {
                    renderer.material = hideMaterial; //ハイライト表示のマテリアルに変更
                }
            }
            else if (hit.collider.gameObject == player)
            {
                renderer.material = defaultMaterial; //デフォルトのマテリアルに戻す
            }
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

    //OnTriggerStay
    void OnTriggerStay(Collider other)
    {
        //氷に触れている場合
        if (other.CompareTag("FrozenArea"))
        {
            onIce = true;
        }
    }

    //OnTriggerExit
    void OnTriggerExit(Collider other)
    {
        //氷から出たら
        if (other.CompareTag("FrozenArea"))
        {
            onIce = false;
            slideVelocity = Vector3.zero; //滑りをリセット
        }
    }

    public static void GameClear()
    {
        GameManager.ToClearState();
        Debug.Log("クリア");
    }
}
