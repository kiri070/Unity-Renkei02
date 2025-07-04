using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScr : MonoBehaviour
{
    private bool GoalOne;
    private bool GoalTwo;

    private bool GoalTreasure; //お宝

    GoalObjManager goalObjManager; //ゴールオブジェクトについているスクリプト
    // Start is called before the first frame update
    void Start()
    {
        GoalOne = false;
        GoalTwo = false;
        GoalTreasure = false;

        goalObjManager = FindObjectOfType<GoalObjManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GoalOne && GoalTwo && GoalTreasure)
        {
           PlayerMover.GameClear();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Goalに触れたら
        if (other.CompareTag("Player1"))
        {
            goalObjManager.touchGoal = true; //エフェクトフラグを立てる
            GoalOne = true;
        }
        if (other.CompareTag("Player2"))
        {
            goalObjManager.touchGoal = true; //エフェクトフラグを立てる
            GoalTwo = true;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj"))
        {
            goalObjManager.touchGoal = true; //エフェクトフラグを立てる
            GoalTreasure = true;
        }
    }
}
