using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMovie : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    void Start()
    {
        
    }
    void Update()
    {
        transform.LookAt(-mainCamera.transform.position);
    }
}
