using UnityEngine;

public class DoubleDoor : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [HideInInspector]
    public bool isOpen = false;

    private float currentLeftAngle = 0f;
    private float currentRightAngle = 0f;

    void Start()
    {
        isOpen = false;
    }

    void Update()
    {
        float targetLeftAngle = isOpen ? openAngle : 0f;
        float targetRightAngle = isOpen ? -openAngle : 0f;

        currentLeftAngle = Mathf.Lerp(currentLeftAngle, targetLeftAngle, Time.deltaTime * openSpeed);
        currentRightAngle = Mathf.Lerp(currentRightAngle, targetRightAngle, Time.deltaTime * openSpeed);

        leftDoor.localRotation = Quaternion.Euler(0, currentLeftAngle, 0);
        rightDoor.localRotation = Quaternion.Euler(0, currentRightAngle, 0);
    }
}
