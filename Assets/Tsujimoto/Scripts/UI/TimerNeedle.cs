using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimerNeedle : MonoBehaviour
{
    RectTransform rectTransform; //針のRectTransform
    GameObject timerImageGroup; //タイマーのグループ
    void Start()
    {
        //コンポーネント取得
        rectTransform = GetComponent<RectTransform>();
        timerImageGroup = GameObject.Find("TimerImageGroup");
    }
    void Update()
    {
        //毎フレームタイマーの針を動かす
        rectTransform.localEulerAngles -= new Vector3(0f, 0f, 0.72f * Time.deltaTime);
    }

    /// <summary>
    /// 残り時間を減少させる場合に呼ぶ関数。
    /// タイマーの針を減少分動かし、揺らします。
    /// </summary>
    public void DecreaseTimerNeedle()
    {
        timerImageGroup.transform.DOShakePosition(0.5f, new Vector3(3, 3), 50); //揺らす
        rectTransform.localEulerAngles -= new Vector3(0f, 0f, 7.2f);
    }
}
