using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScr : MonoBehaviour
{
    private bool GoalOne;
    private bool GoalTwo;

    private bool GoalTreasure; //お宝
    // Start is called before the first frame update
    void Start()
    {
        GoalOne = false;
        GoalTwo = false;
        GoalTreasure = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GoalOne && GoalTwo && GoalTreasure)
        {
           PlayerMover.GameClear();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Goalに触れたら
        if (other.CompareTag("Player1"))
        {
            GoalOne = true;
        }
        if (other.CompareTag("Player2"))
        {
            GoalTwo = true;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj")) GoalTreasure = true;
    }
}
