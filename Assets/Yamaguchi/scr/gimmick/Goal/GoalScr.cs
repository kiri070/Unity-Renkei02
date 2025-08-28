using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScr : MonoBehaviour
{
    private bool GoalOne;
    private bool GoalTwo;

    private bool GoalTreasure; //お宝

    GoalObjManager goalObjManager; //ゴールオブジェクトについているスクリプト
    GameManager gameManager;
    PlayerCnt playerCnt;
    public bool isClearTriggered = false;
    SoundManager soundManager;
    SoundsList soundsList;

    NoticeSystem noticeSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        GoalOne = false;
        GoalTwo = false;
        GoalTreasure = false;

        goalObjManager = FindObjectOfType<GoalObjManager>();
        gameManager = FindObjectOfType<GameManager>();
        playerCnt = FindObjectOfType<PlayerCnt>();
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
        noticeSystem = FindObjectOfType<NoticeSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (GoalOne && GoalTwo && GoalTreasure)
        // {
        //    PlayerMover.GameClear();
        // }

        //ゴールしたら
        if (!isClearTriggered && GoalOne && GoalTwo && GoalTreasure)
        {
            soundManager.OnPlaySE(soundsList.touchGoalSE); //効果音
            goalObjManager.touchGoal = true; //エフェクトフラグを立てる
            isClearTriggered = true; //2回連続で発動しないように
            gameManager.timerActive = false; //タイマーをオフ
            playerCnt.invincible = true; //無敵状態にする
            StartCoroutine(DelayClear(3f)); // 3秒後に実行

            //ゴールの移動位置を渡す
            playerCnt.pos1 = transform.Find("Player1_Pos").gameObject;
            playerCnt.pos2 = transform.Find("Player2_Pos").gameObject;
            playerCnt.treasurePos = transform.Find("Treasure_Pos").gameObject;

            noticeSystem.ActivePanel(noticeSystem.tartgetUI_Goal); //通知を表示
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Goalに触れたら
        if (other.CompareTag("Player1"))
        {
            GoalOne = true;
        }
        if (other.CompareTag("Player2"))
        {
            GoalTwo = true;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj"))
        {
            GoalTreasure = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Goalに触れたら
        if (other.CompareTag("Player1"))
        {
            GoalOne = false;
        }
        if (other.CompareTag("Player2"))
        {
            GoalTwo = false;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj"))
        {
            GoalTreasure = false;
        }
    }

    //クリア状態にするまでに遅延をかける
    IEnumerator DelayClear(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerMover.GameClear();
    }
}
