using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Fadeout_Barrier : MonoBehaviour
{
    [SerializeField] GameObject triggerObj;
    bool isTrigger = false;
    void Update()
    {
        if (!triggerObj.activeSelf && !isTrigger)
        {
            transform.DOScale(Vector3.zero, 1.5f).SetEase(Ease.InBack);
            isTrigger = true;
        }
    }
}
