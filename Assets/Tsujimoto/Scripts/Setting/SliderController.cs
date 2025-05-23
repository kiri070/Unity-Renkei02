using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [Header("カメラ感度のスライダー")]
    public Slider CameraSensitiveSlider;
    // CameraController cameraController; //CameraContorollerのインスタンス
    // void Start()
    // {
    //     cameraController = GameObject.FindObjectOfType<CameraController>(); //CameraControllerのスクリプトを取得
    // }

    // //カメラ感度を適応
    // public void OnCameraSensitivityChange()
    // {
    //     cameraController.sensitivity = CameraSensitiveSlider.value;
    // }
}
