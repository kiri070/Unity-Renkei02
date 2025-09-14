using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomOutSetting : MonoBehaviour
{
    private float zoomout;          // 実際に使う内部フィールド
    [SerializeField] private float zoomoutValue; // Inspectorから設定する値
    [SerializeField] private float zOffsetValue = 0f;   // Z補正

    // プロパティ
    public float Zoomout
    {
        get { return zoomoutValue; }          // 値を返す
        set { zoomout = value; }              // 代入時に処理する
    }

    public float ZOffset
    {
        get { return zOffsetValue; }          // 値を返す
        set { zOffsetValue = value; }         // 代入時に処理する
    }
}
