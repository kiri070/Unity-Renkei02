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

    [Header("宝箱")]
    public GameObject treasure_full;
    public GameObject treasure_half;
    public GameObject treasure_little;
    public GameObject treasure_empty;

    Camera gameCamera;


    void Start()
    {
        //初期位置を記録
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        gameManager = FindObjectOfType<GameManager>();
        playerCnt = FindObjectOfType<PlayerCnt>();
        gameCamera = FindObjectOfType<Camera>();
    }

    void Update()
    {
        ChangeTreasureModel(); //お宝の状態変遷
        OffScreen();           //お宝の画面外検知
        //運ばれていないときはバグ回避のため、ここでも重力をオンにする
        if (rb.velocity == Vector3.zero)
        {
            col.isTrigger = false;
            rb.useGravity = true;
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
    }


    void OnTriggerEnter(Collider other)
    {
        //落ちたら初期位置に戻す
        if (other.CompareTag("DeathArea"))
        {
            gameManager.MinusBoxValue(5);
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

        //魔法に当たったら魔法を消す
        if (other.CompareTag("Majic"))
        {
            //お宝の価値を下げる
            gameManager.MinusBoxValue(10);
            Destroy(other.gameObject);
        }
    }
}
