using System.Collections;
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
    public float moveSpeed = 5f; //移動速度

    [Header("ジャンプ力")]
    [Range(1f, 50f)]
    [SerializeField]
    private float jumpForce; //ジャンプ力

    [Header("弾のクールダウン")]
    public float player1_BulletCoolDown;
    public float player2_BulletCoolDown;

    //弾を発射できるかどうか
    private bool canPlayer1Bullet = true;
    private bool canPlayer2Bullet = true;

    SoundManager soundManager; //SoundManagerのインスタンス
    SoundsList soundsList; //SoundsListのインスタンス

    public Camera[] targetCameras; //カメラの対象設定用の配列 *追加部分

    [Header("弾の着弾地点")]
    [Tooltip("プレイヤー子オブジェクトの着弾地点(状態Hide)")] public GameObject playerFirePoint1;
    [Tooltip("プレイヤー子オブジェクトの着弾地点(状態Hide)")] public GameObject playerFirePoint2;



    void Start()
    {
        //コンポーネント取得
        soundManager = GameObject.FindObjectOfType<SoundManager>();
        soundsList = GameObject.FindObjectOfType<SoundsList>();
    }

    void Update()
    {
        PlayerControlle();
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
            if (Input.GetKeyDown(KeyCode.Space) && mover1.canJump && mover2.canJump)
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

            //弾を発射(長押し:着弾地点表示)
            if (Input.GetKey(KeyCode.F) && canPlayer1Bullet)
            {
                playerFirePoint1.SetActive(true);
            }
            //(離す:発射)
            else if (Input.GetKeyUp(KeyCode.F) && canPlayer1Bullet)
            {
                playerFirePoint1.SetActive(false);
                Instantiate(player1Bullet, player1BulletArea.transform.position, Quaternion.identity);
                //効果音再生
                soundManager.OnPlaySE(soundsList.shotSE);
                StartCoroutine(Player1_BulletCoolDown()); //クールダウン開始
            }

            //弾を発射(長押し:着弾地点表示)
            if (Input.GetKey(KeyCode.H) && canPlayer2Bullet)
            {
                playerFirePoint2.SetActive(true);
            }
            //(離す:発射)
            else if (Input.GetKeyUp(KeyCode.H) && canPlayer2Bullet)
            {
                playerFirePoint2.SetActive(false);
                Instantiate(player2Bullet, player2BulletArea.transform.position, Quaternion.identity);
                //効果音再生
                soundManager.OnPlaySE(soundsList.shotSE);
                StartCoroutine(Player2_BulletCoolDown()); //クールダウン開始
            }

            // //弾を発射
            // if (Input.GetKeyDown(KeyCode.F) && canPlayer1Bullet)
            // {
            //     Instantiate(player1Bullet, player1BulletArea.transform.position, Quaternion.identity);
            //     //効果音再生
            //     soundManager.OnPlaySE(soundsList.shotSE);

            //     StartCoroutine(Player1_BulletCoolDown()); //クールダウン開始
            // }
            // if (Input.GetKeyDown(KeyCode.H) && canPlayer2Bullet)
            // {
            //     Instantiate(player2Bullet, player2BulletArea.transform.position, Quaternion.identity);
            //     //効果音再生
            //     soundManager.OnPlaySE(soundsList.shotSE);
            //     StartCoroutine(Player2_BulletCoolDown()); //クールダウン開始
            // }

            //*追加部分
            if (Input.GetKeyDown(KeyCode.Z)) //のちのちギミックで動かすトリガー
            {
                SwapPlayerControl(); //反転処理実行 
            }
            if (Input.GetKeyDown(KeyCode.C)) //のちのちギミックで動かすトリガー
            {
                RotateCamera(); //反転処理実行
            }
        }
    }

    void SwapPlayerControl() //*追加部分
    {
        //mover1とmover2の中身を入れ替え
        (mover1, mover2) = (mover2, mover1);
        Debug.Log("トリガー検知、プレイヤー操作を反転します");
    }

    void RotateCamera() //*追加部分
    {
        foreach (Camera cam in targetCameras)
        {
            Transform camTransform = cam.transform;

            //X軸に180度回転
            Vector3 currentRotation = camTransform.eulerAngles;
            camTransform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z + 180f);
            Debug.Log("トリガー検知、カメラを反転します");
        }
    }


    //Player1:弾クールダウン
    IEnumerator Player1_BulletCoolDown()
    {
        canPlayer1Bullet = false; //クールタイム処理を走らせる
        yield return new WaitForSeconds(player1_BulletCoolDown);
        canPlayer1Bullet = true;
    }
    //Player2:弾クールダウン
    IEnumerator Player2_BulletCoolDown()
    {
        canPlayer2Bullet = false; //クールタイム処理を走らせる
        yield return new WaitForSeconds(player2_BulletCoolDown);
        canPlayer2Bullet = true;
    }
}