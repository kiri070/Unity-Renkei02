using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTutorial : MonoBehaviour
{
    Tutorial tutorial;

    void Start()
    {
        tutorial = FindObjectOfType<Tutorial>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1")) tutorial.isMove_player1 = true;
        if (other.CompareTag("Player2")) tutorial.isMove_player2 = true;
    }
}
