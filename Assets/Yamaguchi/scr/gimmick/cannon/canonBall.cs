using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canonBall : MonoBehaviour
{
    public float canonx = 200;
    public float canony = 4;

    public float canonz = 0;


    // Start is called before the first frame update
    public void Fire()
    {

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(canonx, canony, canonz), ForceMode.Impulse);
    }

}