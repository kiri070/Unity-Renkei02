using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpToGoal : MonoBehaviour
{
    GameObject player1_pos, player2_pos, treasureBox_pos; //スポーン地点
    GameObject player1, player2, treasureBox; //オブジェクト
    PlayerCnt playerCnt;
    //ステージ2の上下ギミック後,ゴールにワープするかどうか
    bool warpToGoal_Player1 = false;
    bool warpToGoal_Player2 = false;
    bool warpToGoal_TreasureBox = false;
    void Start()
    {
        //ゴール前の各スポーン地点
        treasureBox_pos = GameObject.Find("TreasureBox_GimicGoalPos");
        player1_pos = GameObject.Find("Player1_GimicGoalPos");
        player2_pos = GameObject.Find("Player2_GimicGoalPos");

        //それぞれのオブジェクト
        treasureBox = GameObject.Find("TreasureGroup");
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");

        playerCnt = FindObjectOfType<PlayerCnt>();
    }

    void Update()
    {
        //ワープ魔法陣が起動したら
        if (warpToGoal_Player1 && warpToGoal_Player2 && warpToGoal_TreasureBox)
        {
            playerCnt.OnUnder_OverGimic = false; //上下ギミックをオフ

            // 回転をリセット
            treasureBox.transform.rotation = Quaternion.identity;
            player1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            player2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            //宝箱の運搬中フラグをオフ
            BringObj bringobj = FindObjectOfType<BringObj>();
            bringobj.player1_isBringing = false;
            bringobj.player2_isBringing = false;
            player1.GetComponent<PlayerMover>().heldObject = null; //オブジェクトを空に
            player2.GetComponent<PlayerMover>().heldObject = null; //オブジェクトを空に
            bringobj.GetComponent<Collider>().isTrigger = false; //当たり判定を戻す
            bringobj.GetComponent<Rigidbody>().useGravity = true; //宝箱の重力を戻す

            //転移させる
            treasureBox.transform.position = treasureBox_pos.transform.position;
            player1.transform.position = player1_pos.transform.position;
            player2.transform.position = player2_pos.transform.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //ワープ魔法陣に触れたら
        if (other.CompareTag("Player1")) warpToGoal_Player1 = true;
        if (other.CompareTag("Player2")) warpToGoal_Player2 = true;
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj")) warpToGoal_TreasureBox = true;
    }
    void OnTriggerExit(Collider other)
    {
        //ワープ魔法陣から離れたら
        if (other.CompareTag("Player1")) warpToGoal_Player1 = false;
        if (other.CompareTag("Player2")) warpToGoal_Player2 = false;
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj")) warpToGoal_TreasureBox = false;
    }
}
