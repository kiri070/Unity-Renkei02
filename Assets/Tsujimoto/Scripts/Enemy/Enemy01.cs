using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : MonoBehaviour
{
    [HideInInspector] public EnemyState enemyState; //敵の状態を管理する変数
    [HideInInspector] public GameObject player; //どのプレイヤーが範囲内に入ったか

    Renderer renderer;

    [Header("マテリアル")]
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material moveMaterial;

    [Header("速度")]
    [SerializeField] float speed;

    //敵の状態を管理
    public enum EnemyState
    {
        Idle,
        Move,
    };

    //状態をIdleにする関数
    public void ToEnemyIdle()
    {
        renderer.material = defaultMaterial; //マテリアルをデフォルト状態
        enemyState = EnemyState.Idle;
    }
    //状態をMoveにする関数
    public void ToEnemyMove()
    {
        renderer.material = moveMaterial; //マテリアルを動いている状態
        enemyState = EnemyState.Move;
    }

    void Start()
    {
        //コンポーネント取得
        renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (enemyState == EnemyState.Move)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //ファイヤーに当たったら
        if (other.CompareTag("FireArea"))
        {
            Destroy(gameObject);
        }
        //落下したら
        if (other.CompareTag("DeathArea"))
        {
            Destroy(gameObject);
        }
    }
}
