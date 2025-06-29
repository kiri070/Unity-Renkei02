using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringObj : MonoBehaviour
{
    Vector3 startPos; //初期位置を格納する変数

    void Start()
    {
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathArea"))
        {
            transform.position = startPos;
        }
    }
}
