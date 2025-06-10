using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChecker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        //Player1タグを持つものが触れたらゲームオーバー
        if (other.gameObject.CompareTag("Player1"))
        {
            GameOverManager.becauseGameOver = "異なる色のものに触れてしまった！";
            GameManager.ToGameOverState();
        }
    }
}
