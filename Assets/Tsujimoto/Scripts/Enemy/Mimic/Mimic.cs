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

    Renderer renderer;

    [Header("マテリアル")]
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material attackMaterial;


    [Header("敵本体のコライダー")]
    public Collider bodyCollider;

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

    }

    //魔法攻撃
    void MajicAttack()
    {
        // 向くべき方向を計算（y軸だけ回転）
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0f; // 上下回転しないように

        Debug.Log(player.transform.position);
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
            magic.GetComponent<MajicCnt>().Init(lastPlayerPosition); //関数呼び出して位置を渡す使用

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
            Destroy(gameObject);
        }
        //落下したら
        if (other.CompareTag("DeathArea"))
        {
            Destroy(gameObject);
        }
    }
}
