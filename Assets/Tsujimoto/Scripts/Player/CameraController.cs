using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Header("マウス感度")]
    [Range(0, 1000)]
    public float sensitivity; //マウス感度
    [Header("プレイヤー本体(親)を格納")]
    public Transform playerBody; //プレイヤー本体（親）を入れる

    [Header("マウス上下の制限")]
    [SerializeField]
    [Range(90, 120)]
    [Tooltip("上向き")]
    private float maxVerticalAngle;

    [SerializeField]
    [Range(90, 120)]
    [Tooltip("下向き")]
    private float minVerticalAngle;

    private float verticalRotation = 0f; //マウスのY軸回転の変数

    void Start()
    {
        //カーソル非表示にして固定
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (!Cursor.visible)
        {
            CameraRotate();
        }
    }

    //カメラの回転処理
    void CameraRotate()
    {
        //マウスの移動量を取得
        float x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float y = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //カメラ回転(上下)
        verticalRotation -= y;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, minVerticalAngle); //制限
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); //親オブジェクトを参照して回転

        //横回転は親オブジェクトを回転
        playerBody.Rotate(Vector3.up * x);
    }
}
