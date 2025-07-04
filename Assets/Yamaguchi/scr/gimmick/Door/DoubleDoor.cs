using UnityEngine;

public class DoubleDoor : MonoBehaviour
{
    [Header("ヒンジ（空の親）")]
    public Transform leftHinge;
    public Transform rightHinge;

    [Header("ドア開閉設定")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("左ドア設定")]
    [Tooltip("左ドアの開閉回転方向。+1か-1で指定。")]
    public int leftDirection = 1;

    [Tooltip("左ドアの回転軸")]
    public Vector3 leftRotationAxis = Vector3.up;

    [Header("右ドア設定")]
    [Tooltip("右ドアの開閉回転方向。+1か-1で指定。")]
    public int rightDirection = -1;

    [Tooltip("右ドアの回転軸")]
    public Vector3 rightRotationAxis = Vector3.up;

    [HideInInspector]
    public bool isOpen = false;

    private Quaternion initialLeftHingeRot;
    private Quaternion initialRightHingeRot;

    private float currentLeftAngle = 0f;
    private float currentRightAngle = 0f;

    void Start()
    {
        initialLeftHingeRot = leftHinge.localRotation;
        initialRightHingeRot = rightHinge.localRotation;
    }

    void Update()
    {
        float targetLeftAngle = isOpen ? openAngle * leftDirection : 0f;
        float targetRightAngle = isOpen ? openAngle * rightDirection : 0f;

        currentLeftAngle = Mathf.Lerp(currentLeftAngle, targetLeftAngle, Time.deltaTime * openSpeed);
        currentRightAngle = Mathf.Lerp(currentRightAngle, targetRightAngle, Time.deltaTime * openSpeed);

        leftHinge.localRotation = initialLeftHingeRot * Quaternion.AngleAxis(currentLeftAngle, leftRotationAxis);
        rightHinge.localRotation = initialRightHingeRot * Quaternion.AngleAxis(currentRightAngle, rightRotationAxis);
    }
}
