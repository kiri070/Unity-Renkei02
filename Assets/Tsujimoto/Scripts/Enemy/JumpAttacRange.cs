using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttacRange : MonoBehaviour
{
    Enemy01 enemy01;
    

    void Start()
    {
        //コンポーネント取得
        enemy01 = GetComponentInParent<Enemy01>();
    }
    void OnTriggerEnter(Collider other)
    {
        //プレイヤーがジャンプ攻撃の範囲に入ったら
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            enemy01.enemyState = Enemy01.EnemyState.JumpAttack;
            enemy01.player = other.gameObject;
            Debug.Log("ジャンプ攻撃");
        }
    }
}
