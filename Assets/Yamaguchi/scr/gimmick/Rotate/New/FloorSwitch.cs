//using UnityEngine;

///// <summary>
///// プレイヤーが踏むと連動オブジェクトを動かす床スイッチ
///// </summary>
//public class FloorSwitch : MonoBehaviour
//{
//    [Header("踏んだときに動作させるオブジェクト")]
//    public NewCubeRotator[] cubeRotators;        // 回転するオブジェクト群
//    public NewBlueCubePlatform[] bluePlatforms;  // 移動するオブジェクト群

//    [Header("スイッチが押された時の音")]
//    public AudioSource switchSE;

//    private bool isActivated = false;

//    private void OnTriggerEnter(Collider other)
//    {
//        // プレイヤーが踏んだ場合のみ
//        if (!isActivated && other.CompareTag("Player1")|| other.CompareTag("Player2"))
//        {
//            isActivated = true;

//            // SE再生
//            if (switchSE != null)
//                switchSE.Play();

//            // CubeRotator の動作開始
//            foreach (var rotator in cubeRotators)
//            {
//                if (rotator != null)
//                    rotator.StartRotation();
//            }

//            // BlueCubePlatform の移動開始
//            foreach (var platform in bluePlatforms)
//            {
//                if (platform != null)
//                    platform.TriggerMovement();
//            }
//        }
//    }

//    // スイッチをリセットする場合に呼ぶ
//    public void ResetSwitch()
//    {
//        isActivated = false;

//        foreach (var rotator in cubeRotators)
//        {
//            if (rotator != null)
//                rotator.ResetRotation();
//        }

//        foreach (var platform in bluePlatforms)
//        {
//            if (platform != null)
//                platform.ResetPlatform();
//        }
//    }
//}
