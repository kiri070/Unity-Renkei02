using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTutorial : MonoBehaviour
{
    Tutorial tutorial;
    void Start()
    {
        tutorial = FindObjectOfType<Tutorial>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //�v���C���[���G�ꂽ��t���O�𗧂Ă�
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            tutorial.isColor = true;
        }
    }
}
