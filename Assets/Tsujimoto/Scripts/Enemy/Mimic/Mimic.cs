using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic : MonoBehaviour
{
    [HideInInspector] public EnemyState enemyState; //敵の状態を管理する変数
    [HideInInspector] public GameObject player; //どのプレイヤーが範囲内に入ったか
    [HideInInspector] public Vector3 lastPlayerPosition; //攻撃範囲内に入ったプレイヤーの位置を記録

    [Header("魔法で飛ばすオブジェクト")] public GameObject majicObj;
    [Header("魔法攻撃のクールタイム")][SerializeField] int majicAttackCooltime;
    bool canMajicAttack = true; //魔法を放てるかどうか
    [Header("魔法の速度")][SerializeField] float speed;

    Renderer renderer;

    [Header("マテリアル")]
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material attackMaterial;


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
        renderer.material = defaultMaterial; //マテリアルをデフォルト状態
        enemyState = EnemyState.Idle;
    }
    //状態をMajicAttackにする関数
    public void ToMajicAttack()
    {
        renderer.material = attackMaterial; //マテリアルを攻撃している状態
        enemyState = EnemyState.MajicAttack;
    }

    void Start()
    {
        //コンポーネント取得
        renderer = GetComponent<Renderer>();
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();

    }

    void FixedUpdate()
    {
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
            //魔法を召喚
            GameObject magic = Instantiate(majicObj,transform.position + Vector3.up * 2f,Quaternion.identity);
            // ターゲット座標を渡す
            magic.GetComponent<MagicCnt>().Init(lastPlayerPosition); //関数呼び出し
            //魔法の速度を渡す
            magic.GetComponent<MagicCnt>().SetSpeed(speed); //関数呼び出し

            //効果音再生
            soundManager.OnPlaySE(soundsList.mimicMagicSE);

            canMajicAttack = false;

            //クールタイム処理開始
            StartCoroutine(MajicCookTime());
        }
        
    }
    //クールタイム処理
    IEnumerator MajicCookTime()
    {
        yield return new WaitForSeconds(majicAttackCooltime);
        canMajicAttack = true;
    }


    void OnTriggerEnter(Collider other)
    {
        //ファイヤーに当たったら  //敵本体に当たった場合だけ
        if (other.gameObject.CompareTag("FireArea") && bodyCollider.bounds.Intersects(other.bounds))
        {
            //効果音再生
            soundManager.OnPlaySE(soundsList.killEnemySE);
            
            Destroy(gameObject);
        }
        //落下したら
        if (other.CompareTag("DeathArea"))
        {
            Destroy(gameObject);
        }
    }
}
