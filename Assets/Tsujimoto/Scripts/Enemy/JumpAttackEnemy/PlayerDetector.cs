using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{

    [Header("追従範囲の半径")][SerializeField] private float detectionRadius = 5f; // 追従開始の半径
    [Header("プレイヤーのレイヤー")][SerializeField] private LayerMask playerLayer; // Playerレイヤーを指定（推奨）

    Enemy01 enemy01;

    public Collider[] hits; //hitしたコライダーを格納する配列

    void Start()
    {
        enemy01 = GetComponent<Enemy01>();
    }

    void Update()
    {
        //攻撃をしていたら何もしない
        if (enemy01.enemyState == Enemy01.EnemyState.JumpAttack || enemy01.enemyState == Enemy01.EnemyState.pushAttack) return;

        if (enemy01.enemyState != Enemy01.EnemyState.JumpAttack)
        {
            // 指定した範囲内にプレイヤーがいるか調べる
            hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

            //範囲内なら
            if (hits.Length > 0)
            {
                enemy01.ToEnemyMove();
                enemy01.player = hits[0].gameObject;
            }
            //範囲外なら
            else
            {
                enemy01.ToEnemyIdle();
            }
        }
        
    }

    //範囲を描画(※開発中のみ)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
