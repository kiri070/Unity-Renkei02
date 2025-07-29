using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMagic : MonoBehaviour
{
    float rotateSpeed = 50f;
    void Update()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }
}
