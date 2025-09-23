using UnityEngine;

/// <summary>
/// 回転するオブジェクト
/// </summary>
public class NewCubeRotator : MonoBehaviour
{
    [Header("回転速度（度/秒）")]
    public float rotationSpeed = 90f;

    private bool isRotating = false;
    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (isRotating)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    // スイッチから呼ばれる：回転開始
    public void StartRotation()
    {
        isRotating = true;
    }

    // スイッチから呼ばれる：回転リセット
    public void ResetRotation()
    {
        isRotating = false;
        transform.rotation = initialRotation;
    }
}