using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UntilTheGoal : MonoBehaviour
{
    [Header("ゴールのオブジェクトを格納")]
    public Transform goal;

    [Header("プレイヤーを格納")]
    public Transform player1;
    public Transform player2;
    Vector3 midPoint; //プレイヤー間の距離

    [Header("ゴールまでの距離を表示するスライダー")]
    public Slider slider1;

    void Start()
    {
        midPoint = (player1.position + player2.position) / 2;
        //スライダーの最大値をプレイヤー間の位置に設定
        slider1.maxValue = Vector3.Distance(goal.position, midPoint);
    }

    void Update()
    {
        UntilGoalDistance();
    }

    //ゴールまでの距離を計算して表示
    void UntilGoalDistance()
    {
        midPoint = (player1.position + player2.position) / 2;
        float distance = Vector3.Distance(goal.position, midPoint);
        slider1.value = distance;
    }
}
