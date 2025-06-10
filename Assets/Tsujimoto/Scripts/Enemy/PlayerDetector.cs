using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    Enemy01 enemy01; //Enemy01のインスタンス
    void Start()
    {
        //コンポーネント取得
        enemy01 = GetComponentInParent<Enemy01>();
    }

    //範囲内処理
    void OnTriggerEnter(Collider other)
    {
        //プレイヤーが範囲内に入ったら
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            //敵の状態をMoveに変更
            enemy01.ToEnemyMove();

            //当たったプレイヤーを参照先に渡す
            enemy01.player = other.gameObject;
        }
    }
    //範囲外処理
    void OnTriggerExit(Collider other)
    {
        //プレイヤーが範囲外なら
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            //敵の状態をIdleに変更
            enemy01.ToEnemyIdle();    
        }
    }
}
