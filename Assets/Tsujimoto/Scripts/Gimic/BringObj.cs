using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringObj : MonoBehaviour
{
    Vector3 startPos; //初期位置を格納する変数
    Rigidbody rb;
    Collider col;

    void Start()
    {
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Update()
    {
        //運ばれていないときはバグ回避のため、ここでも重力をオンにする
        if (rb.velocity == Vector3.zero)
        {
            col.isTrigger = false;
            rb.useGravity = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathArea"))
        {
            transform.position = startPos;
        }

        if (other.CompareTag("Majic"))
        {
            Destroy(other.gameObject);
        }
    }
}
