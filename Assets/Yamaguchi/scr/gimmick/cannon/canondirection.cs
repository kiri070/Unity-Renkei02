using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スイッチの役割を持つスクリプト
/// プレイヤーがスイッチに触れたら砲台の首振りをON/OFFする
/// </summary>
public class canondirection : MonoBehaviour
{
    [Header("首振りを制御する砲台（親オブジェクト）")]
    public GameObject _parent;

    private CannonShooter cannon; // 対象のCannonShooterを取得

    void Start()
    {
        // 親オブジェクトからCannonShooterを探す
        cannon = _parent.GetComponent<CannonShooter>();
        if (cannon == null)
        {
            Debug.LogError("CannonShooterが見つかりません。_parentを正しく設定してください。");
        }
    }

    /// <summary>
    /// プレイヤーがスイッチに入ったら首振り開始
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player1") || other.gameObject.CompareTag("Player2"))
        {
            cannon.isSwinging = true;
            Debug.Log("首振り開始");
        }
    }

    /// <summary>
    /// プレイヤーがスイッチから出たら首振り停止
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player1") || other.gameObject.CompareTag("Player2"))
        {
            cannon.isSwinging = false;
            Debug.Log("首振り停止");
        }
    }
}
