using UnityEngine;

public class CubeRotator : MonoBehaviour
{
    [Header("監視する赤キューブたち")]
    public Rigidbody[] redCubeRbs;  // 赤キューブを複数入れられる配列

    [Header("回転させる青キューブ")]
    public Rigidbody blueCubeRb;

    [Header("赤キューブの角速度がこの値を超えたら反応する")]
    public float detectAngularSpeedThreshold = 10f; // deg/s

    [Header("青キューブの回転速度")]
    public float blueCubeRotationSpeedDeg = 30f;    // deg/s

    private bool shouldRotateBlueCube = false;

    void Start()
    {
        blueCubeRb.isKinematic = true;
    }

    void FixedUpdate()
    {
        shouldRotateBlueCube = false;

        // 複数の赤Cubeをチェック
        foreach (Rigidbody redRb in redCubeRbs)
        {
            if (redRb == null) continue;

            // この赤CubeのY軸角速度をdeg/sに変換
            float redYAngularVelocityDeg = redRb.angularVelocity.y * Mathf.Rad2Deg;

            // どれか1つでもしきい値を超えていたら回転フラグON
            if (Mathf.Abs(redYAngularVelocityDeg) > detectAngularSpeedThreshold)
            {
                shouldRotateBlueCube = true;
                break;
            }
        }

        if (shouldRotateBlueCube)
        {
            // フレームあたりの回転量（度）
            float rotationAmount = blueCubeRotationSpeedDeg * Time.fixedDeltaTime;

            // 現在の回転角度取得
            Vector3 currentEuler = blueCubeRb.transform.rotation.eulerAngles;

            // Y軸に回転を足す
            float newY = currentEuler.y + rotationAmount;

            // X軸 -90度を維持しつつ回転を適用
            Quaternion newRot = Quaternion.Euler(-90f, newY, 0f);
            blueCubeRb.transform.rotation = newRot;
        }
    }
}
