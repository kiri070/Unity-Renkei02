using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy01 : MonoBehaviour
{
    [HideInInspector] public EnemyState enemyState; //敵の状態を管理する変数
    [HideInInspector] public GameObject player; //どのプレイヤーが範囲内に入ったか

    //踏みつけ判定
    [Header("踏みつけ判定")][SerializeField] Vector3 boxSize = new Vector3(1f, 0.2f, 1f);
    [SerializeField] Vector3 offset = new Vector3(0f, 0f, 0f);
    [SerializeField] LayerMask playerLayer;
    bool stepOnEnemyCollider = true; //踏むつけ判定のコライダーのオンオフ

    Renderer renderer;

    // [Header("マテリアル")]
    // [SerializeField] Material defaultMaterial;
    // [SerializeField] Material moveMaterial;

    [Header("速度")]
    [SerializeField] float speed;
    Rigidbody rb;

    [Header("ジャンプ攻撃のクールタイム")]
    [SerializeField] int jumpAttackCoolTime;
    [HideInInspector] public bool isJumping = false; //ジャンプ攻撃をするかどうか

    [Header("プッシュ攻撃ステータス")]
    [SerializeField] int pushAttackCoolTime;
    [SerializeField] float pushPower = 70f;
    [HideInInspector] public bool isPushAttack = false; //プッシュ攻撃をするかどうか

    [Header("敵本体のコライダー")]
    public Collider bodyCollider;

    bool jumpAttackToFloor = false; //ジャンプ攻撃後、地面に触れたか
    SoundManager soundManager; //SoundManagerのインスタンス
    SoundsList soundsList; //SoundsListのインスタンス

    [Header("エフェクト")]
    [Tooltip("ジャンプ攻撃の着地時")] public GameObject smork;
    [Tooltip("踏まれた時")] public GameObject step;
    [Tooltip("死んだ時")] public GameObject killed;
    [Tooltip("突進攻撃のチャージ")] public GameObject chargePushAttackEffect;

    [HideInInspector] public bool canJumpAttack = true;


    //敵の状態を管理
    public enum EnemyState
    {
        Idle,
        Move,
        JumpAttack,
        pushAttack,
    };

    //状態をIdleにする関数
    public void ToEnemyIdle()
    {
        // renderer.material = defaultMaterial; //マテリアルをデフォルト状態
        enemyState = EnemyState.Idle;
    }
    //状態をMoveにする関数
    public void ToEnemyMove()
    {
        // renderer.material = moveMaterial; //マテリアルを動いている状態
        enemyState = EnemyState.Move;
    }
    //状態をJumpAttackにする関数
    public void ToEnemyJumpAttack()
    {
        if (!canJumpAttack) return;

        // renderer.material = moveMaterial;
        enemyState = EnemyState.JumpAttack;
        canJumpAttack = false;
        StartCoroutine(ResetJumpAttackCoolDown());
        // StartCoroutine(ResetJumpAttackCoolDown());
    }

    //状態をPushAttackにする関数
    public void ToEnemyPushAttack()
    {
        // renderer.material = moveMaterial; //マテリアルを動いている状態
        enemyState = EnemyState.pushAttack;
    }

    void Start()
    {
        //コンポーネント取得
        renderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        soundManager = GameObject.FindObjectOfType<SoundManager>();
        soundsList = GameObject.FindObjectOfType<SoundsList>();
    }

    void Update()
    {
        StepOnEnemy();

        //プッシュ攻撃が終了したら踏みつけ判定をアクティブ
        if (!stepOnEnemyCollider && rb.velocity == Vector3.zero)
        {
            stepOnEnemyCollider = true;
        }
    }

    void FixedUpdate()
    {
        Debug.Log("敵の状態:" + enemyState);
        switch (enemyState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Moving();
                break;
            case EnemyState.JumpAttack:
                JumpAttack();
                break;
            case EnemyState.pushAttack:
                PushAttack();
                break;
        }
    }

    //アイドル処理関数
    void Idle()
    {
        Debug.Log("Idle状態");
    }

    //移動処理関数
    void Moving()
    {
        if (enemyState == EnemyState.Move && player != null)
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
            jumpAttackToFloor = true; //着地した時に効果音を再生する用
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

    //プッシュ攻撃
    void PushAttack()
    {
        if (!isPushAttack)
        {
            StartCoroutine(PreparePushAttack(1f, 0.3f));
        }
    }

    //プレイヤーに触れまれたら
    void StepOnEnemy()
    {
        Vector3 center = transform.position + Vector3.up * 1f;
        Collider[] hits = Physics.OverlapBox(center + offset, boxSize / 2, Quaternion.identity, playerLayer);
        if (hits.Length > 0 && stepOnEnemyCollider)
        {
            //上から踏まれていない場合はreturn
            if (hits[0].gameObject.GetComponent<Rigidbody>().velocity.y > -0.1f) return;

            PlayerMover pm = FindObjectOfType<PlayerMover>();
            pm.OnStepEnemy(); //音をプレイヤー側で鳴らす
            soundManager.OnPlaySE(soundsList.stepOnPlayer);
            Instantiate(step, transform.position, step.transform.rotation); //エフェクト再生
            //キルエフェクト再生
            Instantiate(killed, transform.position, killed.transform.rotation);
            hits[0].gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            hits[0].gameObject.GetComponent<Rigidbody>().AddForce(0f, 50f, 0f, ForceMode.Impulse); //プレイヤーを跳ねさせる
            Destroy(gameObject);
        }
        
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + Vector3.up * 1f + offset;
        Gizmos.DrawWireCube(center, boxSize);
    }

    void OnTriggerEnter(Collider other)
    {
        //ファイヤーに当たったら  //敵本体に当たった場合だけ
        if (other.gameObject.CompareTag("FireArea") && bodyCollider.bounds.Intersects(other.bounds))
        {
            //効果音再生
            soundManager.OnPlaySE(soundsList.killEnemySE);
            //エフェクト再生
            Instantiate(killed, transform.position, killed.transform.rotation);
            Destroy(gameObject);
        }
        //落下したら
        if (other.CompareTag("DeathArea"))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor") && jumpAttackToFloor)
        {
            //効果音再生
            soundManager.OnPlaySE(soundsList.jumpAttack);

            //エフェクト再生
            Instantiate(smork, transform.position, Quaternion.identity);

            jumpAttackToFloor = false;
        }
    }

    //ジャンプ攻撃のクールタイム
    IEnumerator ResetJumpAttackCoolDown()
    {
        yield return new WaitForSeconds(jumpAttackCoolTime);
        canJumpAttack = true;
    }

    //Push攻撃の準備
    IEnumerator PreparePushAttack(float duration, float magnitude)
    {

        isPushAttack = true;
        float elapsed = 0f;
        Vector3 originalPos = transform.position;

        //チャージエフェクトを再生
        GameObject chargeEffect = Instantiate(chargePushAttackEffect, transform.position, chargePushAttackEffect.transform.rotation);
        //指定の秒数左右に振動
        while (elapsed < duration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            transform.position = originalPos + new Vector3(offsetX, 0, 0);

            //方向を取得
            Vector3 direction1 = (player.transform.position - transform.position).normalized;
            direction1.y = 0f; // 上下回転しないように

            //回転処理
            if (direction1 != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction1);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        //チャージエフェクトを削除
        Destroy(chargeEffect);

        transform.position = originalPos;

        //方向を取得
        Vector3 direction2 = (player.transform.position - transform.position).normalized;
        direction2.y = 0f; //上には飛ばないように

        rb.AddForce(direction2 * pushPower, ForceMode.Impulse); //プッシュ攻撃
        stepOnEnemyCollider = false; //一時、踏みつけ判定をオフ
        StartCoroutine(EndPushAttack()); //プッシュ攻撃終了後の処理
        StartCoroutine(WaitToMove(2f)); //指定秒数止まる
    }

    //ジャンプ攻撃が終了したら
    IEnumerator EndJumpAttack()
    {
        yield return new WaitForSeconds(jumpAttackCoolTime);
        //クールタイムが終了したら
        isJumping = false; //ジャンプフラグをfalse
        canJumpAttack = true;
        // ToEnemyMove();
    }

    //プッシュ攻撃が終了したら
    IEnumerator EndPushAttack()
    {
        yield return new WaitForSeconds(pushAttackCoolTime);
        isPushAttack = false;
    }

    //攻撃の後、秒数指定でMove状態に戻す(何もしない時間を作る)
    IEnumerator WaitToMove(float time)
    {
        yield return new WaitForSeconds(time);
        ToEnemyMove();
    }
}
