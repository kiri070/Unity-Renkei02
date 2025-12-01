using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameTimeEnemyJudge : MonoBehaviour
{
    [Header("同時踏み相方（複数可）")]
    [Tooltip("同時踏み付け処理のもう一方の敵を格納")] public SameTimeEnemyJudge[] partners;

    [HideInInspector] public bool isStepped = false;
    Enemy01 enemy;

    [HideInInspector] public List<GameObject> playerObj;
    void Start()
    {
        enemy = GetComponent<Enemy01>();
        playerObj = new List<GameObject>();
    }

    public void OnStepped(GameObject player)
    {
        if (isStepped) return;

        isStepped = true;

        // 踏んだプレイヤーを追加（1回だけになる）
        if (!playerObj.Contains(player))
        {
            playerObj.Add(player);
        }

        // 全パートナーが踏まれている？
        bool allStepped = true;
        foreach (var p in partners)
        {
            if (p == null || p.isStepped == false)
            {
                allStepped = false;
                break;
            }
        }

        // 条件達成 → 全員同時に Kill
        if (allStepped)
        {
            KillAll();
        }
    }

    public void OnReleased(GameObject player)
    {
        if (playerObj.Contains(player))
            playerObj.Remove(player);

        if (playerObj.Count == 0)
            isStepped = false;   // 踏んでない状態に戻す
    }

    void KillAll()
    {
        // 自分
        Kill();

        // 全パートナー
        foreach (var p in partners)
        {
            if (p != null)
            {
                p.Kill();
            }
        }
    }

    public void Kill()
    {
        var scripts = GetComponent<Enemy01>();
        foreach (var player in playerObj)
        {
            scripts.SameTimeKillEffect(player, true);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathArea"))
        {
            foreach (var p in partners)
            {
                if (p != null)
                {
                    var scripts = GetComponent<Enemy01>();
                    foreach (var player in playerObj)
                    {
                        scripts.SameTimeKillEffect(player, false);
                    }
                    Destroy(p.gameObject);
                }
            }
        }
    }
}
