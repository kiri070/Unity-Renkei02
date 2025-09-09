using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beltconveyor : MonoBehaviour
{

    [Header("ベルトコンベアーの速度")]public float speed = 5f;
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player1") || other.gameObject.CompareTag("Player2") || other.gameObject.layer == LayerMask.NameToLayer("BringObj"))
        {
            Debug.Log(other.gameObject.name);
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * speed, ForceMode.Acceleration);
        }
    }
}
