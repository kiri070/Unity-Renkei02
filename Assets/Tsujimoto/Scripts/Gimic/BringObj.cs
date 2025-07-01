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

    void Start()
    {
        //初期位置を記録
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        gameManager = FindObjectOfType<GameManager>();
        playerCnt = FindObjectOfType<PlayerCnt>();
    }

    void Update()
    {
        //運ばれていないときはバグ回避のため、ここでも重力をオンにする
        if (rb.velocity == Vector3.zero)
        {
            col.isTrigger = false;
            rb.useGravity = true;
        }
    }

    //初期位置に箱を戻す関数
    public void ReSpawnBox()
    {
        transform.position = startPos;
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
