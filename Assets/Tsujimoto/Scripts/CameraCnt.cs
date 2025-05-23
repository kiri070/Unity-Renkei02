using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCnt : MonoBehaviour
{
    [Header("カメラの移動速度")]
    [Range(1, 10)]
    [SerializeField]
    private float cameraMoveSpeed; //カメラの移動速度
    void Update()
    {
        //前方に進む
        transform.position += new Vector3(0f, 0f, cameraMoveSpeed) * Time.deltaTime;
    }
}
