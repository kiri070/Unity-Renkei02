using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderOverGimic_Manager : MonoBehaviour
{
    bool player1_Ongimic = false; //プレイヤー1の上下ギミックフラグ
    bool player2_Ongimic = false; //プレイヤー2の上下ギミックフラグ
    bool treasureBox_Ongimic = false; //宝箱の上下ギミックフラグ
    bool gimicTrigger = false; //ギミックのトリガー
    public GameObject gimicCamera, mainCamera; //それぞれのカメラ

    void Start()
    {
        //それぞれのカメラを取得
        // mainCamera = GameObject.Find("MainCamera");
        // gimicCamera = GameObject.Find("GimicCamera");
    }

    void Update()
    {
        //上下ギミックに乗ったら
        if (player1_Ongimic && player2_Ongimic && treasureBox_Ongimic && !gimicTrigger)
        {
            gimicTrigger = true; //トリガーをオン
            PlayerCnt playerCnt = FindObjectOfType<PlayerCnt>();
            playerCnt.OnUnder_OverGimic = true; //上下ギミック起動フラグを立てる

            //カメラを切り替える
            gimicCamera.SetActive(true);
            mainCamera.SetActive(false);
            
            // gimicCamera.GetComponent<Camera>().enabled = true;

            WarpToGimic(); //上下ギミックにスポーンさせる
        }
    }

    //上下ギミックにスポーンさせる関数
    void WarpToGimic()
    {
        //それぞれのスポーンポイントを取得
        Vector3 player1_SpawnPos = GameObject.Find("Player1_GimicSpawnPos").transform.position;
        Vector3 player2_SpawnPos = GameObject.Find("Player2_GimicSpawnPos").transform.position;
        Vector3 treasureBox_SpawnPos = GameObject.Find("TreasureBox_GimicSpawnPos").transform.position;

        //それぞれのオブジェクトを取得
        GameObject player1 = GameObject.Find("Player1");
        GameObject player2 = GameObject.Find("Player2");
        GameObject treasureBox = GameObject.Find("TreasureGroup");

        //宝箱の運搬中フラグをオフ
        BringObj bringobj = FindObjectOfType<BringObj>();
        bringobj.player1_isBringing = false;
        bringobj.player2_isBringing = false;

        //Rigidbodyの位置を動かす
        Rigidbody rb1 = player1.GetComponent<Rigidbody>();
        Rigidbody rb2 = player2.GetComponent<Rigidbody>();
        Rigidbody rb3 = treasureBox.GetComponent<Rigidbody>();
        rb1.velocity = Vector3.zero;
        rb2.velocity = Vector3.zero;
        rb3.velocity = Vector3.zero;
        rb1.position = player1_SpawnPos;
        rb2.position = player2_SpawnPos;
        rb3.position = treasureBox_SpawnPos;

        // //それぞれのスポーンポイントにスポーン(旧)
        // player1.transform.position = player1_SpawnPos;
        // player2.transform.position = player2_SpawnPos;
        // treasureBox.transform.position = treasureBox_SpawnPos;

        PlayerCnt playerCnt = FindObjectOfType<PlayerCnt>();
        //プレイヤー,宝箱を天井と地面に設定(スポーンは別)
        playerCnt.ChangeTopBottom(player1, player2, treasureBox, true);


        // //最初はプレイヤー1を天井判定にする
        // player1.GetComponent<PlayerMover>().onRoof = true;
        // //宝箱の運搬中フラグをオフ
        // BringObj bringobj = FindObjectOfType<BringObj>();
        // bringobj.player1_isBringing = false;
        // bringobj.player2_isBringing = false;
        // player1.GetComponent<PlayerMover>().heldObject = null; //オブジェクトを空に
        // player2.GetComponent<PlayerMover>().heldObject = null; //オブジェクトを空に
        // bringobj.GetComponent<Collider>().isTrigger = false; //当たり判定を戻す
    }
    private void OnTriggerEnter(Collider other)
    {
        //上下ギミック起動装置に乗った時
        if (other.CompareTag("Player1")) player1_Ongimic = true;
        if (other.CompareTag("Player2")) player2_Ongimic = true;
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj")) treasureBox_Ongimic = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //上下ギミック起動装置から離れた時
        if (other.CompareTag("Player1")) player1_Ongimic = false;
        if (other.CompareTag("Player2")) player2_Ongimic = false;
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj")) treasureBox_Ongimic = false;
    }
}
