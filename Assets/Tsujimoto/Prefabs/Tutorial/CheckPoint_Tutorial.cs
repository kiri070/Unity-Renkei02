using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint_Tutorial : MonoBehaviour
{
    Tutorial tutorial;
    void Start()
    {
        tutorial = FindObjectOfType<Tutorial>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーが触れたらフラグを立てる
        if(other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            tutorial.isCheckPoint = true;
        }
    }
}
