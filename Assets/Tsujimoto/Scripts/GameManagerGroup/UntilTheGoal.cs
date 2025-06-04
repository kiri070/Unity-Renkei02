using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UntilTheGoal : MonoBehaviour
{
    [Header("ゴールのオブジェクトを格納")]
    public Transform goal;

    [Header("カメラを格納")]
    public Transform player1;
    public Transform player2;

    [Header("ゴールまでの距離を表示するスライダー")]
    public Slider slider1;
    public Slider slider2;

    void Start()
    {
        //スライダー1の最大値をプレイヤー1の位置に設定
        slider1.maxValue = Vector3.Distance(goal.position, player1.position);

        //スライダー2の最大値をプレイヤー2の位置に設定
        slider2.maxValue = Vector3.Distance(goal.position, player2.position);
    }

    void Update()
    {
        UntilGoalDistance();
    }

    //ゴールまでの距離を計算して表示
    void UntilGoalDistance()
    {
        //プレイヤー1
        float distance = Vector3.Distance(goal.position, player1.position);
        slider1.value = distance;

        //プレイヤー2
        float distance2 = Vector3.Distance(goal.position, player2.position);
        slider2.value = distance2;
    }
}
