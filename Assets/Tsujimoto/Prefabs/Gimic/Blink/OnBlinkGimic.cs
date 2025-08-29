using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBlinkGimic : MonoBehaviour
{
    [Header("同時に点滅させるオブジェクト")] [SerializeField] List<GameObject> blinkObj;

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーが触れたら同時にオブジェクトを点滅開始
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            foreach (var obj in blinkObj)
            {
                BlinkAndDestroy blinking = obj.GetComponent<BlinkAndDestroy>();
                blinking.StartBlinking();    
            }
        }
    }
}
