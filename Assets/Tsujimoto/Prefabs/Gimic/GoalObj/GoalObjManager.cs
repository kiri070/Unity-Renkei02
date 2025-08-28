using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalObjManager : MonoBehaviour
{
    [Header("回転させる柱")] public GameObject pillar;
    [Header("回転させるcube")] public GameObject[] rotateCube = new GameObject[4];
    [Header("エフェクト")] public GameObject effect1;
    public bool touchGoal; //ゴールに触れたら

    Vector3[] cubeStartPos; // 初期位置を保存する配列
    public float moveAmplitude = 0.5f; // 上下に動く幅
    public float moveSpeed = 2f; // 動く速さ

    void Start()
    {
        touchGoal = false;

        // 初期位置を保存
        cubeStartPos = new Vector3[rotateCube.Length];
        for (int i = 0; i < rotateCube.Length; i++)
        {
            cubeStartPos[i] = rotateCube[i].transform.position;
        }
    }
    void Update()
    {
        // 回転させる
        pillar.transform.Rotate(0f, 100f * Time.deltaTime, 0f);

        for (int i = 0; i < rotateCube.Length; i++)
        {
            // 回転
            rotateCube[i].transform.Rotate(0f, 200f * Time.deltaTime, 0f);

            // 上下にゆっくり動かす
            Vector3 pos = cubeStartPos[i];
            pos.y += Mathf.Sin(Time.time * moveSpeed + i) * moveAmplitude;
            rotateCube[i].transform.position = pos;
        }

        // ゴールしたらエフェクトを再生
        if (touchGoal)
        {
            effect1.SetActive(true);
        }
    }
}
