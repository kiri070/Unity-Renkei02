using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCnt : MonoBehaviour
{
    InputCnt controls; //入力を受け取るためのもの
    InputCnt controls1; // Player1用
    InputCnt controls2; // Player2用

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

    [Header("スタート地点")]
    public GameObject player1_StartPoint;
    public GameObject player2_StartPoint;

    [Header("エフェクト")]
    [Tooltip("スポーンエフェクト")][SerializeField] GameObject spawnEffect;
    GameObject player1_SpawnEffectPoint, player2_SpawnEffectPoint; //スポーンエフェクトの発生位置
    [HideInInspector] public GameObject currentCheckPoint; //最新のチェックポイント
    public bool invincible;//プレイヤーが無敵時間かどうか


    //弾を発射できるかどうか
    private bool canPlayer1Bullet = true;
    private bool canPlayer2Bullet = true;

    //荷物を持っているかどうか
    [HideInInspector] public bool isPlayer1BringObj = false;
    [HideInInspector] public bool isPlayer2BringObj = false;


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

        //エフェクトの位置を取得
        player1_SpawnEffectPoint = GameObject.Find("Player1_SpawnEffectPoint");
        player2_SpawnEffectPoint = GameObject.Find("Player2_SpawnEffectPoint");

        //シングルプレイ用
        controls = new InputCnt(); 
        //マルチプレイ用
        controls1 = new InputCnt();
        controls2 = new InputCnt();

        //一部の入力イベントを登録
        RegisterEvents();

        //コントローラーのみ
        if (Gamepad.all.Count >= 2)
        {
            controls1.devices = new InputDevice[] { Gamepad.all[0] };
            controls2.devices = new InputDevice[] { Gamepad.all[1] };
        }
        //コントローラーとキーボード
        else if(Gamepad.all.Count >= 1)
        {
            controls1.devices = new InputDevice[] { Gamepad.all[0] };
            controls2.devices = new InputDevice[] { Keyboard.current };
        }
        //キーボード + コントローラー
        //if (Gamepad.all.Count >= 1)
        //{
        //    controls1.devices = new InputDevice[] { Gamepad.all[0] };
        //    controls2.devices = new InputDevice[] { Keyboard.current };
        //}
    }

    void Update()
    {
        //ゲームモードがシングルなら
        if (GameManager.gameMode == GameManager.GameMode.SinglePlayer)
        {
            controls.Enable(); // 入力を有効にする
            controls1.Player1.Disable();
            controls2.Player2.Disable();
        }
        //ゲームモードがマルチプレイヤーなら
        else if (GameManager.gameMode == GameManager.GameMode.MultiPlayer)
        {
            controls.Disable();
            controls1.Player1.Enable();
            controls2.Player2.Enable();
        }

        //操作
        if (GameManager.gameMode == GameManager.GameMode.SinglePlayer) //シングルプレイ時
            PlayerControlle();
        else if (GameManager.gameMode == GameManager.GameMode.MultiPlayer) //マルチプレイ時
            MultiPlayerControlle();

    }
    void RegisterEvents()
    {
        //ジャンプ
        controls1.Player1.Jump.performed += OnPlayer1Jump;
        controls2.Player2.Jump.performed += OnPlayer2Jump;

        //Player1:荷物を持つ
        controls1.Player1.Bring.started += ctx =>
        {
            if (GameManager.state != GameManager.GameState.Playing) return;
            isPlayer1BringObj = true;
        };
        controls1.Player1.Bring.canceled += ctx =>
        {
            if (GameManager.state != GameManager.GameState.Playing) return;
            isPlayer1BringObj = false;
        };
        //Player2:荷物を持つ
        controls2.Player2.Bring.started += ctx =>
        {
            if (GameManager.state != GameManager.GameState.Playing) return;
            isPlayer2BringObj = true;
        };
        controls2.Player2.Bring.canceled += ctx =>
        {
            if (GameManager.state != GameManager.GameState.Playing) return;
            isPlayer2BringObj = false;
        };

        //Player1:スライディング開始
        controls1.Player1.Sliding.started += ctx =>
        {
            if (GameManager.state != GameManager.GameState.Playing) return;
            mover1.StartSliding();
            mover2.StartSliding();
        };
        //Player1:スライディング終了
        controls1.Player1.Sliding.canceled += ctx =>
        {
            if (GameManager.state != GameManager.GameState.Playing) return;
            mover1.EndSliding();
            mover2.EndSliding();
        };
        //Player2:スライディング開始
        controls2.Player2.Sliding.started += ctx =>
        {
            if (GameManager.state != GameManager.GameState.Playing) return;
            mover1.StartSliding();
            mover2.StartSliding();
        };
        //Player2:スライディング終了
        controls2.Player2.Sliding.canceled += ctx =>
        {
            if (GameManager.state != GameManager.GameState.Playing) return;
            mover1.EndSliding();
            mover2.EndSliding();
        };
    }
    //Player1:ジャンプ(マルチ用)
    void OnPlayer1Jump(InputAction.CallbackContext ctx)
    {
        if (mover1 != null && mover1.canJump && mover2 != null && mover2.canJump)
        {
            mover1.jumpForce = jumpForce;
            mover1.jumping = true;

            mover2.jumpForce = jumpForce;
            mover2.jumping = true;

            soundManager.OnPlaySE(soundsList.jumpSE);
        }
    }
    //Player2:ジャンプ(マルチ用)
    void OnPlayer2Jump(InputAction.CallbackContext ctx)
    {
        if (mover2 != null && mover2.canJump && mover1 != null && mover1.canJump)
        {
            mover1.jumpForce = jumpForce;
            mover1.jumping = true;

            mover2.jumpForce = jumpForce;
            mover2.jumping = true;
            soundManager.OnPlaySE(soundsList.jumpSE);
        }
    }

    //シングルプレイ用
    void PlayerControlle()
    {
        if (GameManager.state == GameManager.GameState.Playing)
        {
            //Player1:移動(キーボードとコントローラー対応)
            Vector2 input1 = controls.Player.Move1.ReadValue<Vector2>();
            Vector3 moveDir1 = new Vector3(input1.x, 0f, input1.y); // y→Z軸へ
            mover1.Assignment(moveDir1 * moveSpeed);

            //Player2:移動(キーボードとコントローラー対応)
            Vector2 input2 = controls.Player.Move2.ReadValue<Vector2>();
            Vector3 moveDir2 = new Vector3(input2.x, 0f, input2.y); // y→Z軸へ
            mover2.Assignment(moveDir2 * moveSpeed);


            //スライディング開始
            controls.Player.Sliding.started += ctx =>
            {
                mover1.StartSliding();
                mover2.StartSliding();
            };
            //スライディング終了
            controls.Player.Sliding.canceled += ctx =>
            {
                mover1.EndSliding();
                mover2.EndSliding();
            };

            //ジャンプ入力
            controls.Player.Jump.performed += ctx =>
            {
                if (mover1.canJump && mover2.canJump)
                {
                    mover1.jumpForce = this.jumpForce;
                    mover1.jumping = true;

                    mover2.jumpForce = this.jumpForce;
                    mover2.jumping = true;

                    soundManager.OnPlaySE(soundsList.jumpSE);
                }
            };

            //Player1:荷物を持つ
            controls.Player.Bring1.started += ctx =>
            {
                if (GameManager.state != GameManager.GameState.Playing) return;
                isPlayer1BringObj = true;
            };
            controls.Player.Bring1.canceled += ctx =>
            {
                if (GameManager.state != GameManager.GameState.Playing) return;
                isPlayer1BringObj = false;
            };
            //Player2:荷物を持つ
            controls.Player.Bring2.started += ctx =>
            {
                if (GameManager.state != GameManager.GameState.Playing) return;
                isPlayer2BringObj = true;
            };
            controls.Player.Bring2.canceled += ctx =>
            {
                if (GameManager.state != GameManager.GameState.Playing) return;
                isPlayer2BringObj = false;
            };


            //// Player1: WASD
            //float x1 = Input.GetAxisRaw("Horizontal");
            //float z1 = Input.GetAxisRaw("Vertical");
            //Vector3 input1 = new Vector3(x1, 0, z1) * moveSpeed;
            //mover1.Assignment(input1);

            //// Player2: IJKL
            //float x2 = Input.GetAxisRaw("Horizontal_P2");
            //float z2 = Input.GetAxisRaw("Vertical_P2");
            //Vector3 input2 = new Vector3(x2, 0, z2) * moveSpeed;
            //mover2.Assignment(input2);

            ////ジャンプ(共通)
            //if (Input.GetKeyDown(KeyCode.Space) && mover1.canJump && mover2.canJump)
            //{
            //    mover1.jumpForce = this.jumpForce;
            //    mover1.jumping = true;

            //    mover2.jumpForce = this.jumpForce;
            //    mover2.jumping = true;

            //    //ジャンプSE
            //    soundManager.OnPlaySE(soundsList.jumpSE);
            //}

            ////Shiftを押している時はスライディング開始
            //if (Input.GetKeyDown(KeyCode.LeftShift))
            //{
            //    mover1.StartSliding();
            //    mover2.StartSliding();
            //}
            ////Shiftを離したらスライディング終了
            //else if (Input.GetKeyUp(KeyCode.LeftShift))
            //{
            //    mover1.EndSliding();
            //    mover2.EndSliding();
            //}

            ////弾を発射(長押し:着弾地点表示)
            //if (Input.GetKey(KeyCode.F) && canPlayer1Bullet)
            //{
            //    playerFirePoint1.SetActive(true);
            //}
            ////(離す:発射)
            //else if (Input.GetKeyUp(KeyCode.F) && canPlayer1Bullet)
            //{
            //    playerFirePoint1.SetActive(false);
            //    Instantiate(player1Bullet, player1BulletArea.transform.position, Quaternion.identity);
            //    //効果音再生
            //    soundManager.OnPlaySE(soundsList.shotSE);
            //    StartCoroutine(Player1_BulletCoolDown()); //クールダウン開始
            //}

            ////弾を発射(長押し:着弾地点表示)
            //if (Input.GetKey(KeyCode.H) && canPlayer2Bullet)
            //{
            //    playerFirePoint2.SetActive(true);
            //}
            ////(離す:発射)
            //else if (Input.GetKeyUp(KeyCode.H) && canPlayer2Bullet)
            //{
            //    playerFirePoint2.SetActive(false);
            //    Instantiate(player2Bullet, player2BulletArea.transform.position, Quaternion.identity);
            //    //効果音再生
            //    soundManager.OnPlaySE(soundsList.shotSE);
            //    StartCoroutine(Player2_BulletCoolDown()); //クールダウン開始
            //}


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
    //マルチプレイ用
    void MultiPlayerControlle()
    {
        if (GameManager.state == GameManager.GameState.Playing)
        {
            //Player1:移動(キーボードとコントローラー対応)
            Vector2 input1 = controls1.Player1.Move.ReadValue<Vector2>();
            Vector3 moveDir1 = new Vector3(input1.x, 0f, input1.y); // y→Z軸へ
            mover1.Assignment(moveDir1 * moveSpeed);

            //Player2:移動(キーボードとコントローラー対応)
            Vector2 input2 = controls2.Player2.Move.ReadValue<Vector2>();
            Vector3 moveDir2 = new Vector3(input2.x, 0f, input2.y); // y→Z軸へ
            mover2.Assignment(moveDir2 * moveSpeed);
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

    //スタート地点にスポーンさせる関数
    public void SpwanStartPoint()
    {
        //スポーンエフェクト再生
        Instantiate(spawnEffect, player1_SpawnEffectPoint.transform.position, spawnEffect.transform.rotation);
        Instantiate(spawnEffect, player2_SpawnEffectPoint.transform.position, spawnEffect.transform.rotation);
        
        mover1.transform.position = player1_StartPoint.transform.position;
        mover2.transform.position = player2_StartPoint.transform.position;
        StartCoroutine(InvincibleTimer());
    }
    //チェックポイントにスポーンさせる関数
    public void SpawnCheckPoint()
    {
        //スポーンポイントを格納
        GameObject spawn1 = currentCheckPoint.transform.Find("Player1_Spawn").gameObject; 
        GameObject spawn2 = currentCheckPoint.transform.Find("Player2_Spawn").gameObject;
        //スポーンエフェクト再生
        Instantiate(spawnEffect, spawn1.transform.position, spawnEffect.transform.rotation);
        Instantiate(spawnEffect, spawn2.transform.position, spawnEffect.transform.rotation);

        mover1.transform.position = spawn1.transform.position;
        mover2.transform.position = spawn2.transform.position;

        StartCoroutine(InvincibleTimer());
    }
    //無敵時間管理
    IEnumerator InvincibleTimer()
    {
        invincible = true; //無敵状態に
        yield return new WaitForSeconds(1f);
        invincible = false; //解除
    }

    //入力イベントを削除
    public void OnDestroyEvents()
    {
        controls?.Dispose();
        controls1?.Dispose();
        controls2?.Dispose();
    }
}