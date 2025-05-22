using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("マウス感度")]
    [Range(0, 1000)]
    public float sensitivity; //マウス感度
    [Header("プレイヤー本体(親)を格納")]
    public Transform playerBody; //プレイヤー本体（親）を入れる
    private float verticalRotation = 0f; //マウスのY軸回転の変数

    void Start()
    {
        //カーソル非表示にして固定
        Cursor.lockState = CursorLockMode.Locked; 
    }
    void Update()
    {
        CameraRotate();
    }

    //カメラの回転処理
    void CameraRotate()
    {
        //マウスの移動量を取得
        float x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float y = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //カメラ回転(上下)
        verticalRotation -= y;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); //制限
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); //親オブジェクトを参照して回転

        //横回転は親オブジェクトを回転
        playerBody.Rotate(Vector3.up * x); 
    }
}
