using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_PlayerDetector : MonoBehaviour
{
    [Header("追従範囲の半径")][SerializeField] private float detectionRadius = 5f; // 追従開始の半径
    [Header("プレイヤーのレイヤー")][SerializeField] private LayerMask playerLayer; // Playerレイヤーを指定（推奨）


    Mimic mimic;

    public Collider[] hits; //hitしたコライダーを格納する配列

    void Start()
    {
        mimic = GetComponent<Mimic>();
    }

    void Update()
    {
        // 指定した範囲内にプレイヤーがいるか調べる
        hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        //範囲内なら
        if (hits.Length > 0)
        {
            mimic.ToMajicAttack();
            mimic.player = hits[0].gameObject;

            //プレイヤーの位置を記録
            mimic.lastPlayerPosition = hits[0].transform.position;
        }
        //範囲外なら
        else
        {
            mimic.ToIdle();
        }
    }

    //範囲を描画(開発中のみ)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
