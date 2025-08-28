using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic : MonoBehaviour
{
    [HideInInspector] public EnemyState enemyState; //敵の状態を管理する変数
    [HideInInspector] public GameObject player; //どのプレイヤーが範囲内に入ったか
    [HideInInspector] public Vector3 lastPlayerPosition; //攻撃範囲内に入ったプレイヤーの位置を記録

    [Header("踏みつけ判定")][SerializeField] Vector3 boxSize = new Vector3(1f, 0.2f, 1f);
    [SerializeField] Vector3 offset;
    [SerializeField] LayerMask playerLayer;

    [Header("魔法で飛ばすオブジェクト")] public GameObject majicObj;
    [Header("魔法攻撃のクールタイム")][SerializeField] int majicAttackCooltime;
    bool canMajicAttack = true; //魔法を放てるかどうか
    [Header("魔法の速度")][SerializeField] float speed;

    [Header("エフェクト")]
    [Tooltip("死んだ時")] public GameObject killed;
    [Tooltip("踏まれた時")] public GameObject step;
    [Tooltip("チャージ")] public GameObject chargeEffectPrefab;

    Renderer renderer;

    PlayerMover mover1, mover2; //プレイヤーのスクリプト

    [Header("敵本体のコライダー")]
    public Collider bodyCollider;

    SoundManager soundManager; //SoundManagerのインスタンス
    SoundsList soundsList; //SoundListのインスタンス

    //敵の状態を管理
    public enum EnemyState
    {
        Idle,
        MajicAttack,
    };

    //状態をIdleにする関数
    public void ToIdle()
    {
        enemyState = EnemyState.Idle;
    }
    //状態をMajicAttackにする関数
    public void ToMajicAttack()
    {
        enemyState = EnemyState.MajicAttack;
    }

    void Start()
    {
        //コンポーネント取得
        renderer = GetComponent<Renderer>();
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();

        GameObject player1 = GameObject.Find("Player1");
        mover1 = player1.GetComponent<PlayerMover>();
        GameObject player2 = GameObject.Find("Player2");
        mover2 = player2.GetComponent<PlayerMover>();
    }

    void FixedUpdate()
    {
        StepOnEnemy();
        
        switch (enemyState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.MajicAttack:
                MajicAttack();
                break;
        }
    }

    //アイドル処理関数
    void Idle()
    {
        Debug.Log("Idle状態");
    }

    //魔法攻撃
    void MajicAttack()
    {
        // 向くべき方向を計算（y軸だけ回転）
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0f; // 上下回転しないように

        //回転処理
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }

        //魔法を召喚
        if (canMajicAttack)
        {
            StartCoroutine(ChargeAndShoot());
        }
        
    }

    //チャージエフェクト後に魔法を撃つ
    IEnumerator ChargeAndShoot()
    {
        canMajicAttack = false;

        // チャージ用のエフェクトを生成
        GameObject chargeEffect = Instantiate(chargeEffectPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);

        // エフェクトの再生時間分待機
        float chargeTime = 1.5f; //1.5秒待つ
        yield return new WaitForSeconds(chargeTime);

        // チャージエフェクトを消去
        Destroy(chargeEffect);

        // 魔法を生成して発射
        GameObject magic = Instantiate(majicObj, transform.position + Vector3.up * 2f, Quaternion.identity);
        magic.GetComponent<MagicCnt>().Init(lastPlayerPosition);
        magic.GetComponent<MagicCnt>().SetSpeed(speed);

        // 発射音
        soundManager.OnPlaySE(soundsList.mimicMagicSE);

        // クールタイム処理開始
        StartCoroutine(MajicCookTime());
    }
    //クールタイム処理
    IEnumerator MajicCookTime()
    {
        yield return new WaitForSeconds(majicAttackCooltime);
        canMajicAttack = true;
    }

    //プレイヤーに触れまれたら
    void StepOnEnemy()
    {
        Vector3 center = transform.position + Vector3.up * 1f;
        Collider[] hits = Physics.OverlapBox(center + offset, boxSize / 2, transform.rotation, playerLayer);
        if (hits.Length > 0)
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
        Vector3 center = transform.position + transform.up * 1f + offset;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
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
}
