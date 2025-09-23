using UnityEngine;

/// <summary>
/// ��]����I�u�W�F�N�g
/// </summary>
public class NewCubeRotator : MonoBehaviour
{
    [Header("��]���x�i�x/�b�j")]
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

    // �X�C�b�`����Ă΂��F��]�J�n
    public void StartRotation()
    {
        isRotating = true;
    }

    // �X�C�b�`����Ă΂��F��]���Z�b�g
    public void ResetRotation()
    {
        isRotating = false;
        transform.rotation = initialRotation;
    }
}