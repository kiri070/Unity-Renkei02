using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [Header("エフェクト")]
    public GameObject checkpointWaveEffect;
    public GameObject checkPointEffect; //炎

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            if (!checkPointEffect.activeSelf)
            {
                Vector3 pos = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
                Instantiate(checkpointWaveEffect, pos, checkpointWaveEffect.transform.rotation);
            }
            
        }
    }
}
