using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCnt : MonoBehaviour
{
    GameObject player1, player2; //プレイヤーのオブジェクト
    Vector3 baseOffset; // 中心からの初期オフセット
    Quaternion baseRotation; //初期回転値

    [Header("カメラの広がる値")]
    [Range(0.1f, 0.5f)]
    [SerializeField] float distanceFactor = 0.2f;

    float fixedCenterY; //カメラのy値

    void Start()
    {
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");

        // プレイヤーの中心を基準にオフセットを計算
        Vector3 center = (player1.transform.position + player2.transform.position) / 2f;
        baseOffset = transform.position - center;

        //カメラの初期回転値
        baseRotation = transform.rotation;
        fixedCenterY = center.y;
    }

    void Update()
    {
        //プレイヤー同志の中間地点を計算
        Vector3 center = (player1.transform.position + player2.transform.position) / 2f;
        center.y = fixedCenterY; //ジャンプで高さが変更されないように

        //プレイヤー同士の横距離の絶対値
        float horizontalDistance = Mathf.Abs(player1.transform.position.x - player2.transform.position.x);

        //Vector3(0f, 横距離 * 倍率, -横距離 * 倍率)
        Vector3 zoomOffset = new Vector3(0f, horizontalDistance * distanceFactor, -horizontalDistance * distanceFactor);

        // 中心に追従 + オフセット + ズーム補正
        transform.position = center + baseOffset + zoomOffset;
        transform.rotation = baseRotation;
    }
}
