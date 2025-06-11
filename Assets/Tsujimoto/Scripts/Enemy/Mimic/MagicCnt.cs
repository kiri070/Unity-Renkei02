using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCnt : MonoBehaviour
{

    [Header("魔法のスピード")][HideInInspector] public float speed = 10f;
    private Vector3 targetPos;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //飛ばす位置を取得
    public void Init(Vector3 targetPosition)
    {
        targetPos = targetPosition;
    }
    //速度を取得
    public void SetSpeed(float x)
    {
        speed = x;
    }
    void Update()
    {
        //targetPosまで飛ばす
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        Vector3 direction = transform.position - targetPos;
        if (direction == Vector3.zero)
        {
            Destroy(gameObject);
        }
    }
}
