using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RotateLoadingImage : MonoBehaviour
{
    void Update()
    {
        //画像をロード中に回転させる
        transform.Rotate(0f, 0f, -200f * Time.deltaTime);
    }
}
