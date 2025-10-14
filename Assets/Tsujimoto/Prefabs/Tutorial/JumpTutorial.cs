using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTutorial : MonoBehaviour
{
    Tutorial tutorial;
    void Start()
    {
        tutorial = FindObjectOfType<Tutorial>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            tutorial.jumpArea = true; //ジャンプエリアフラグ
        }
    }
}
