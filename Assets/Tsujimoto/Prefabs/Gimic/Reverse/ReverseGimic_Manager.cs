using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseGimic_Manager : MonoBehaviour
{
    Vector3 player1Pos, player2Pos, treasureBoxPos; //スポーンポイント
    GameObject player1Obj, player2Obj, treasureBoxObj; //オブジェクト
    [HideInInspector]public bool onPlayer1, onPlayer2, onTreasureBox; //ギミックに触れているかどうか
    void Start()
    {
        //反転ギミックのスポーンポイント
        player1Pos = GameObject.Find("Player1_GimicReverseSpawnPos").transform.position;
        player2Pos = GameObject.Find("Player2_GimicReverseSpawnPos").transform.position;
        treasureBoxPos = GameObject.Find("TreasureBox_GimicReverseSpawnPos").transform.position;

        //オブジェクトを取得
        player1Obj = GameObject.Find("Player1");
        player2Obj = GameObject.Find("Player2");
        treasureBoxObj = GameObject.Find("TreasureGroup");
    }
    void Update()
    {
        //ギミックが起動したら
        if (onPlayer1 && onPlayer2 && onTreasureBox)
        {
            PlayerCnt playerCnt = FindObjectOfType<PlayerCnt>();
            playerCnt.ChangeTopBottom(player2Obj, player1Obj, treasureBoxObj, true); //パラメーターを反転させる

            //Rigidbodyの位置を動かす
            Rigidbody rb1 = player1Obj.GetComponent<Rigidbody>();
            Rigidbody rb2 = player2Obj.GetComponent<Rigidbody>();
            Rigidbody rb3 = treasureBoxObj.GetComponent<Rigidbody>();
            rb1.velocity = Vector3.zero;
            rb2.velocity = Vector3.zero;
            rb3.velocity = Vector3.zero;
            rb1.position = player1Pos;
            rb2.position = player2Pos;
            rb3.position = treasureBoxPos;
        }
    }
}
