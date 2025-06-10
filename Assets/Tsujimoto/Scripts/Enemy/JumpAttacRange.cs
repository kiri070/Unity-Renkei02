using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttacRange : MonoBehaviour
{
    [Header("サイズ")][SerializeField] Vector3 boxSize = new Vector3(1, 1, 2); //全体サイズ
    [Header("位置調整")][SerializeField] float boxForwardOffset = 1.5f; //前方へのずらし距離
    [Header("プレイヤーのレイヤー")][SerializeField] LayerMask playerLayer;

    Enemy01 enemy01;
    PlayerDetector playerDetector;

    void Start()
    {
        enemy01 = GetComponent<Enemy01>();
        playerDetector = GetComponent<PlayerDetector>();
    }

    void Update()
    {
        //Box中心を前方にオフセット
        Vector3 center = transform.position + transform.forward * boxForwardOffset;

        //回転を合わせる(Boxが敵の向きにそう)
        Quaternion rotation = transform.rotation;

        //半分のサイズを渡す
        Vector3 halfExtents = boxSize * 0.5f;

        // 指定した範囲内にプレイヤーがいるか調べる
        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation, playerLayer);

        //ジャンプ攻撃の範囲内なら
        if (hits.Length > 0)
        {
            enemy01.ToEnemyJumpAttack();
            enemy01.player = hits[0].gameObject;
        }
        //ジャンプ攻撃の範囲外かつ、追従範囲内なら
        else if(playerDetector.hits.Length > 0)
        {
            //Move状態に戻す
            enemy01.ToEnemyMove();
        }
    }

    //範囲を描画(※開発中のみ)
    void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + transform.forward * boxForwardOffset;
        Quaternion rotation = transform.rotation;
        Vector3 halfExtents = boxSize * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
