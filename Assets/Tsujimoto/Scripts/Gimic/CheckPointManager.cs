using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [Header("エフェクト")]
    public GameObject checkpointWaveEffect;
    public GameObject checkPointEffect; //炎

    [HideInInspector] public bool isActive = false;

    //NoticeSystem noticeSystem;

    private void Start()
    {
        //noticeSystem = FindObjectOfType<NoticeSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            if (!checkPointEffect.activeSelf)
            {
                Vector3 pos = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
                Instantiate(checkpointWaveEffect, pos, checkpointWaveEffect.transform.rotation);
                // CheckPointArea を非アクティブにする
                Transform area = transform.Find("CheckPointArea");
                area.gameObject.SetActive(false);

                //noticeSystem.ActivePanel(noticeSystem.targetUI_CheckPoint); //チェックポイント画面演出
            }

        }
    }
}
