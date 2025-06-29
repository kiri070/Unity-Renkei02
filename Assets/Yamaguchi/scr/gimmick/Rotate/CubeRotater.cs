using UnityEngine;

public class CubeRotator : MonoBehaviour
{
    public Rigidbody redCubeRb;  // Rigidbody に変更
    public Rigidbody blueCubeRb;

    public float torqueAmount = 50f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // ぶつかったら Y軸回転トルクを与える
            Vector3 torque = Vector3.up * torqueAmount;
            redCubeRb.AddTorque(torque, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // redCube の回転を blueCube にコピー
        blueCubeRb.MoveRotation(redCubeRb.rotation);
    }
}
