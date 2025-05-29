using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerCnt : MonoBehaviour
{
    [Header("プレイヤー格納")]
    public PlayerMover mover1;
    public PlayerMover mover2;

    [Header("弾が発射されるエリア")]
    public GameObject player1BulletArea;
    public GameObject player2BulletArea;
    [Header("弾")]
    public GameObject player1Bullet;
    public GameObject player2Bullet;

    [Header("移動速度")]
    [Range(1f, 10f)]
    [SerializeField]
    private float moveSpeed = 5f; //移動速度

    [Header("ジャンプ力")]
    [Range(1f, 50f)]
    [SerializeField]
    private float jumpForce; //ジャンプ力

    [Header("弾のクールダウン")]
    public float player1_BulletCoolDown;
    public float player2_BulletCoolDown;

    //弾のクールダウンを計算するための変数
    private float player1CurrentCoolDown;
    private float player2CurrentCoolDown;

    //弾を発射できるかどうか
    private bool canPlayer1Bullet = true;
    private bool canPlayer2Bullet = true;

    SoundManager soundManager; //SoundManagerのインスタンス
    SoundsList soundsList; //SoundsListのインスタンス


    void Start()
    {
        //コンポーネント取得
        soundManager = GameObject.FindObjectOfType<SoundManager>();
        soundsList = GameObject.FindObjectOfType<SoundsList>();
    }

    void Update()
    {
        PlayerControlle();

        //弾を発射したらクールタイム処理
        if (!canPlayer1Bullet)
            Player1_BulletCoolDown();
        if (!canPlayer2Bullet)
            Player2_BulletCoolDown();
    }

    void PlayerControlle()
    {
        if (GameManager.state == GameManager.GameState.Playing)
        {
            // Player1: WASD
            float x1 = Input.GetAxisRaw("Horizontal");
            float z1 = Input.GetAxisRaw("Vertical");
            Vector3 input1 = new Vector3(x1, 0, z1) * moveSpeed;
            mover1.Assignment(input1);

            // Player2: IJKL
            float x2 = Input.GetAxisRaw("Horizontal_P2");
            float z2 = Input.GetAxisRaw("Vertical_P2");
            Vector3 input2 = new Vector3(x2, 0, z2) * moveSpeed;
            mover2.Assignment(input2);

            //ジャンプ(共通)
            if (Input.GetKey(KeyCode.Space) && mover1.canJump && mover2.canJump)
            {
                mover1.jumpForce = this.jumpForce;
                mover1.jumping = true;

                mover2.jumpForce = this.jumpForce;
                mover2.jumping = true;

                //ジャンプSE
                soundManager.OnPlaySE(soundsList.jumpSE);
            }

            //Shiftを押している時はスライディング開始
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                mover1.StartSliding();
                mover2.StartSliding();
            }
            //Shiftを離したらスライディング終了
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                mover1.EndSliding();
                mover2.EndSliding();
            }

            //弾を発射
            if (Input.GetKeyDown(KeyCode.F) && canPlayer1Bullet)
            {
                Instantiate(player1Bullet, player1BulletArea.transform.position, Quaternion.identity);
                canPlayer1Bullet = false; //クールタイム処理を走らせる
            }
            if (Input.GetKeyDown(KeyCode.H) && canPlayer2Bullet)
            {
                Instantiate(player2Bullet, player2BulletArea.transform.position, Quaternion.identity);
                canPlayer2Bullet = false; //クールタイム処理を走らせる
            }
        }
    }

    //Player1の弾発射クールタイム計算
    void Player1_BulletCoolDown()
    {
        player1CurrentCoolDown += Time.deltaTime;
        if (player1_BulletCoolDown <= player1CurrentCoolDown)
        {
            canPlayer1Bullet = true;
            player1CurrentCoolDown = 0f;
        }
    }
    //Player2の弾発射クールタイム計算
    void Player2_BulletCoolDown()
    {
        player2CurrentCoolDown += Time.deltaTime;
        if (player2_BulletCoolDown <= player2CurrentCoolDown)
        {
            canPlayer2Bullet = true;
            player2CurrentCoolDown = 0f;
        }
    }
}