using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseObj : MonoBehaviour
{
    ReverseGimic_Manager rm;

    void Start()
    {
        rm = FindObjectOfType<ReverseGimic_Manager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //ギミック起動フラグを立てる
        if (other.CompareTag("Player1")) rm.onPlayer1 = true;
        if (other.CompareTag("Player2")) rm.onPlayer2 = true;
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj")) rm.onTreasureBox = true;
    }
    private void OnTriggerExit(Collider other)
    {
        //ギミック起動フラグをオフ
        if (other.CompareTag("Player1")) rm.onPlayer1 = false;
        if (other.CompareTag("Player2")) rm.onPlayer2 = false;
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj")) rm.onTreasureBox = false;
    }
}
