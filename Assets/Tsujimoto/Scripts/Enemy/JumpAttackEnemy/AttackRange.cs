using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    [Header("サイズ")][SerializeField] Vector3 boxSize = new Vector3(1, 1, 2); //全体サイズ
    [Header("位置調整（前後・左右・上下）")]
    [SerializeField] Vector3 boxOffset = new Vector3(0, 0, 1.5f); // x:左右, y:上下, z:前後
    [Header("位置調整")][SerializeField] float boxForwardOffset = 1.5f; //前方へのずらし距離
    [Header("プレイヤーのレイヤー")][SerializeField] LayerMask playerLayer;

    Enemy01 enemy01;
    PlayerDetector playerDetector;

    [Header("Push攻撃判定の範囲")][SerializeField] float pushRange = 3f;

    void Start()
    {
        enemy01 = GetComponent<Enemy01>();
        playerDetector = GetComponent<PlayerDetector>();
    }

    // void Update()
    // {
    //     // 移動 or 攻撃中どちらでも判定できるようにする（ただしIdleは除外）
    //     if (enemy01.enemyState == Enemy01.EnemyState.Idle || enemy01.enemyState == Enemy01.EnemyState.pushAttack) return;

    //     Vector3 center = transform.position + transform.forward * boxForwardOffset;
    //     Quaternion rotation = transform.rotation;
    //     Vector3 halfExtents = boxSize * 0.5f;

    //     Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation, playerLayer);

    //     if (hits.Length > 0)
    //     {
    //         GameObject target = hits[0].gameObject;
    //         float distance = Vector3.Distance(transform.position, target.transform.position);

    //         enemy01.player = target;

    //         // Pushの距離ならPush優先
    //         if (distance < pushRange)
    //         {
    //             enemy01.ToEnemyPushAttack();
    //         }
    //         // Pushでなく、ジャンプ攻撃が許可されていれば
    //         else if (enemy01.canJumpAttack)
    //         {
    //             enemy01.ToEnemyJumpAttack();
    //         }
    //     }
    //     else if (playerDetector.hits.Length > 0)
    //     {
    //         enemy01.ToEnemyMove();
    //     }
    // }

    // //範囲を描画(開発中のみ)
    // void OnDrawGizmosSelected()
    // {
    //     Vector3 center = transform.position + transform.forward * boxForwardOffset;
    //     Quaternion rotation = transform.rotation;
    //     Vector3 halfExtents = boxSize * 0.5f;

    //     // 全体の攻撃範囲 (Jumpも含む)
    //     Gizmos.color = new Color(1f, 0f, 0f, 0.2f); // 半透明赤
    //     Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
    //     Gizmos.DrawCube(Vector3.zero, boxSize); // filled cube
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(Vector3.zero, boxSize); // wireframe

    //     // Push範囲（中心からの距離で判定してるのでSphereで表示）
    //     Gizmos.matrix = Matrix4x4.identity; // 座標戻す
    //     Gizmos.color = new Color(0f, 1f, 1f, 0.3f); // 半透明シアン
    //     Gizmos.DrawSphere(transform.position, pushRange);
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawWireSphere(transform.position, pushRange);
    // }

    void Update()
    {
        if (enemy01.enemyState == Enemy01.EnemyState.Idle || enemy01.enemyState == Enemy01.EnemyState.pushAttack) return;

        // ← offsetをローカル座標系で適用する
        Vector3 center = transform.position + transform.rotation * boxOffset;
        Quaternion rotation = transform.rotation;
        Vector3 halfExtents = boxSize * 0.5f;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation, playerLayer);

        if (hits.Length > 0)
        {
            GameObject target = hits[0].gameObject;
            float distance = Vector3.Distance(transform.position, target.transform.position);

            enemy01.player = target;

            if (distance < pushRange)
            {
                enemy01.ToEnemyPushAttack();
            }
            else if (enemy01.canJumpAttack)
            {
                enemy01.ToEnemyJumpAttack();
            }
        }
        else if (playerDetector.hits.Length > 0)
        {
            enemy01.ToEnemyMove();
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + transform.rotation * boxOffset;
        Quaternion rotation = transform.rotation;
        Vector3 halfExtents = boxSize * 0.5f;

        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, boxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, pushRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pushRange);
    }
}
