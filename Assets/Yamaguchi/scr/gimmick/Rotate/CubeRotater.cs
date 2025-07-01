using UnityEngine;

public class CubeRotator : MonoBehaviour
{
    public Rigidbody redCubeRb;
    public Rigidbody blueCubeRb;

    public float detectAngularSpeedThreshold = 10f; // deg/s
    public float blueCubeRotationSpeedDeg = 30f;    // deg/s

    private bool shouldRotateBlueCube = false;

    void Start()
    {
        blueCubeRb.isKinematic = true;
    }

    void FixedUpdate()
    {
        // redCube の Y軸角速度（deg/s）
        float redYAngularVelocityDeg = redCubeRb.angularVelocity.y * Mathf.Rad2Deg;

        shouldRotateBlueCube = Mathf.Abs(redYAngularVelocityDeg) > detectAngularSpeedThreshold;

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
