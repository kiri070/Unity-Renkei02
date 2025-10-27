using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 矢印を動かすスクリプト
/// </summary>
public class ArrowUI : MonoBehaviour
{
    void Start()
    {
        var startY = transform.localPosition.y; //y座標を取得
        transform.DOLocalMoveY(startY + 1f, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine); //上下に動かす
    }
}
