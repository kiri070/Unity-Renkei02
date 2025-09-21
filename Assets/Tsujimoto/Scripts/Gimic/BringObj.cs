    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BringObj : MonoBehaviour
{
    Vector3 startPos; //初期位置を格納する変数
    Rigidbody rb;
    Collider col;
    GameManager gameManager;
    PlayerCnt playerCnt;

    public bool player1_isBringing = false; //宝箱が運ばれているか
    public bool player2_isBringing = false; //宝箱が運ばれているか

    //ステージ2の上下ギミック時の位置
    public bool top = false;
    public bool bottom = false;

    [Header("宝箱")]
    public GameObject treasure_full;
    public GameObject treasure_half;
    public GameObject treasure_little;
    public GameObject treasure_empty;

    [Header("エフェクト")]
    [Tooltip("攻撃を食らった時")] public GameObject damegedEffect;
    [Tooltip("宝箱を運んでいない時")] public GameObject boxPos_Effeect;

    Camera gameCamera;
    CameraCnt cameraCnt;

    SoundManager soundManager;
    SoundsList soundsList;
    void Start()
    {
        //初期位置を記録
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        gameManager = FindObjectOfType<GameManager>();
        playerCnt = FindObjectOfType<PlayerCnt>();
        gameCamera = FindObjectOfType<Camera>();
        cameraCnt = FindObjectOfType<CameraCnt>();
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
    }

    void Update()
    {
        //上下ギミック起動中に天井フラグが立ったら
        if (playerCnt.OnUnder_OverGimic && top && !bottom)
        {
            rb.useGravity = false; //重力をオフ
            rb.AddForce(Vector3.up * 70f, ForceMode.Acceleration); //擬似的な重力を上方向に作る
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        //上下ギミック起動中に地面フラグが立ったら
        else if (playerCnt.OnUnder_OverGimic && bottom && !top)
        {
            rb.useGravity = true; //重力をオン
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        ChangeTreasureModel(); //お宝の状態変遷
        // OffScreen();           //お宝の画面外検知
        //運ばれていないときはバグ回避のため、ここでも重力をオンにする
        if (rb.velocity == Vector3.zero)
        {
            col.isTrigger = false;
            rb.useGravity = true;
        }

        //宝箱が運ばれていない時
        if (!player1_isBringing && !player2_isBringing)
        {
            boxPos_Effeect.SetActive(true);
        }
        //運ばれている時
        else
        {
            boxPos_Effeect.SetActive(false);
        }
    }

    //状態に応じてモデルの入れ替え
    void ChangeTreasureModel()
    {
        treasure_full.SetActive(false);
        treasure_half.SetActive(false);
        treasure_little.SetActive(false);
        treasure_empty.SetActive(false);

        if (gameManager.boxValue >= 80)
            treasure_full.SetActive(true);
        else if (gameManager.boxValue >= 50)
            treasure_half.SetActive(true);
        else if (gameManager.boxValue >= 10)
            treasure_little.SetActive(true);
        else
            treasure_empty.SetActive(true);
    }

    //初期位置に箱を戻す関数
    public void ReSpawnBox()
    {
        transform.position = startPos;
    }

    //画面外検知
    void OffScreen()
    {
        //このオブジェクトをカメラの画面上での位置に変換
        Vector3 viewPos = gameCamera.WorldToViewportPoint(transform.position);

        //画面外に出た時
        if (viewPos.x < 0 || viewPos.x > 1 ||
           viewPos.y < 0 || viewPos.y > 1 ||
           viewPos.z < 0)
        {
            //通常
            if (!playerCnt.OnUnder_OverGimic)
            {
                if (!playerCnt.currentCheckPoint)
                {
                    //タイマー減少
                    gameManager.DecreaseTimer(gameManager.decreaseFallTimer);

                    //スタート地点に戻る
                    playerCnt.SpwanStartPoint();
                    gameManager.MinusBoxValue(5);
                }
                //チェックポイントがあったら
                else
                {
                    //タイマー減少
                    gameManager.DecreaseTimer(gameManager.decreaseFallTimer);

                    playerCnt.SpawnCheckPoint();
                    gameManager.MinusBoxValue(5);
                }
            }
            //上下ギミック中
            else if (playerCnt.OnUnder_OverGimic)
            {
                //タイマー減少
                gameManager.DecreaseTimer(gameManager.decreaseFallTimer);

                //スタート地点に戻る
                playerCnt.SpwanStartPoint_Gimic();
                gameManager.MinusBoxValue(5);
            }
            
        }
    }


    void OnTriggerEnter(Collider other)
    {
        //落ちたら初期位置に戻す
        if (other.CompareTag("DeathArea"))
        {
            gameManager.MinusBoxValue(5);
            //効果音
            soundManager.OnPlaySE(soundsList.treasureDamagedSE);
            //チェックポイントがある場合
            if (playerCnt.currentCheckPoint != null)
            {
                playerCnt.SpawnCheckPoint();

                // 動くオブジェクトをリセットする
                foreach (TriggeredMovingPlatform platform in FindObjectsOfType<TriggeredMovingPlatform>())
                {
                    Debug.Log("Reset対象: " + platform.name);
                    platform.ResetPlatform();
                }
            }
            //チェックポイントがない場合
            else
            {
                //初期位置にスポーン
                playerCnt.SpwanStartPoint();

                // 動くオブジェクトをリセットする
                foreach (TriggeredMovingPlatform platform in FindObjectsOfType<TriggeredMovingPlatform>())
                {
                    Debug.Log("Reset対象: " + platform.name);
                    platform.ResetPlatform();
                }
            }
        }

        //画面外なら
        if (other.CompareTag("GameOverWall"))
        {
            gameManager.MinusBoxValue(5);
            //効果音
            soundManager.OnPlaySE(soundsList.treasureDamagedSE);
            //チェックポイントがある場合
            if (playerCnt.currentCheckPoint != null)
            {
                playerCnt.SpawnCheckPoint();
            }
            //チェックポイントがない場合
            else
            {
                //初期位置にスポーン
                playerCnt.SpwanStartPoint();
            }
        }

        //上下ギミック中の画面外判定なら
        if (other.CompareTag("GameOverWall_Gimic") && !playerCnt.invincible)
        {
            soundManager.OnPlaySE(soundsList.fallSE); //SE
            //タイマー減少
            gameManager.DecreaseTimer(gameManager.decreaseFallTimer);
            //お宝の価値を減少
            gameManager.MinusBoxValue(5);

            //スタート地点に戻る
            if (!playerCnt.currentCheckPoint_TopBottom) playerCnt.SpwanStartPoint_Gimic(); //チェックポイントがない場合
            else if (playerCnt.currentCheckPoint_TopBottom) playerCnt.SpwanCheckPoint_Gimic(); //チェックポイントがある場合
        }

        //魔法に当たったら魔法を消す
        if (other.CompareTag("Majic"))
        {
            //お宝の価値を下げる
            gameManager.MinusBoxValue(10);
            Destroy(other.gameObject);

            //効果音
            soundManager.OnPlaySE(soundsList.treasureDamagedSE);
            //エフェクト
            if (gameManager.boxValue > 0)
            {
                Instantiate(damegedEffect, transform.position, damegedEffect.transform.rotation);
            }
        }
        //大砲の球に当たったら
        else if (other.CompareTag("CannonBall"))
        {
            //お宝の価値を下げる
            gameManager.MinusBoxValue(10);
            StartCoroutine(cameraCnt.ShakeCamera(0.7f, 0.3f)); //カメラを揺らす)
            Destroy(other.gameObject);

            //効果音
            soundManager.OnPlaySE(soundsList.treasureDamagedSE);
            //エフェクト
            if (gameManager.boxValue > 0)
            {
                Instantiate(damegedEffect, transform.position, damegedEffect.transform.rotation);
            }

        }
    }
}
