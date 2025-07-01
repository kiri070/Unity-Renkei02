using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyonPyon : MonoBehaviour
{
    Rigidbody rb;
    public float jumpForce = 50f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bound"))
        {
            rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
        }
    }
}
