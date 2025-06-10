using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    Rigidbody rb;

    [Header("ジャンプ攻撃のクールタイム")]
    [SerializeField] int jumpAttackCoolTime;
    bool isJumping = false; //ジャンプ攻撃をするかどうか

    [Header("敵本体のコライダー")]
    public Collider bodyCollider;

    //敵の状態を管理
    public enum EnemyState
    {
        Idle,
        Move,
        JumpAttack,
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
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //アイドル
        Idle();
        //移動
        Moving();
        //ジャンプ攻撃
        JumpAttack();
    }

    //アイドル処理関数
    void Idle()
    {
        
    }

    //移動処理関数
    void Moving()
    {
        if (enemyState == EnemyState.Move)
        {
            //移動処理
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            // 向くべき方向を計算（y軸だけ回転）
            Vector3 direction = (player.transform.position - transform.position).normalized;
            direction.y = 0f; // 上下回転しないように

            //回転処理
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
            }
        }
    }

    //ジャンプ攻撃関数
    public void JumpAttack()
    {
        if (enemyState == EnemyState.JumpAttack && !isJumping)
        {
            isJumping = true;

            // //プレイヤーの方向へジャンプ
            // Vector3 jumpDirection = (player.transform.position - transform.position).normalized;
            // jumpDirection.y = 5f; //上方向に跳ねる力

            // rb.AddForce(jumpDirection * 10f, ForceMode.Impulse);

            // 水平方向への力
            Vector3 horizontal = (player.transform.position - transform.position);
            horizontal.y = 0f; // 水平成分だけ取り出す
            horizontal = horizontal.normalized * 3f; // 水平方向のジャンプ強さ

            // 上方向の力
            Vector3 vertical = Vector3.up * 50f; // ジャンプの高さ

            // 合成してジャンプ
            Vector3 jumpForce = horizontal + vertical;
            rb.AddForce(jumpForce, ForceMode.Impulse);

            //落下
            if (rb.velocity.y < 0f && isJumping)
            {
                rb.AddForce(Vector3.down * 7f, ForceMode.Acceleration);
            }
            //クールタイム処理
            StartCoroutine(EndJumpAttack());
        }
    }

    //ジャンプ攻撃が終了したら
    IEnumerator EndJumpAttack()
    {
        yield return new WaitForSeconds(jumpAttackCoolTime);
        //クールタイムが終了したら
        isJumping = false; //ジャンプフラグをfalse
    }
    void OnTriggerEnter(Collider other)
    {
        //ファイヤーに当たったら  //敵本体に当たった場合だけ
        if (other.gameObject.CompareTag("FireArea") && bodyCollider.bounds.Intersects(other.bounds))
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
