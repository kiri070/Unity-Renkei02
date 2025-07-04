using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    // スイッチを踏んだときに反応させるプレイヤーを選択
    public enum PlayerType
    {
        Player1, // Player1 のみ
        Player2, // Player2 のみ
        Both     // Player1 も Player2 も有効
    }

    [Header("どのプレイヤーが踏んだら反応するか")]
    public PlayerType playerType;

    [Header("開閉させるドアリスト")]
    public List<DoubleDoor> doors = new List<DoubleDoor>();

    // プレイヤーがスイッチに入ったとき
    private void OnTriggerEnter(Collider other)
    {
        if (IsValidPlayer(other))
        {
            foreach (var door in doors)
            {
                door.isOpen = true; // ドアを開く
                Debug.Log("扉を開きます");
            }
        }
    }

    // プレイヤーがスイッチから出たとき
    private void OnTriggerExit(Collider other)
    {
        if (IsValidPlayer(other))
        {
            foreach (var door in doors)
            {
                door.isOpen = false; // ドアを閉じる
            }
        }
    }

    // プレイヤーが有効かどうかを判定
    private bool IsValidPlayer(Collider other)
    {
        switch (playerType)
        {
            case PlayerType.Player1:
                return other.CompareTag("Player1");
            case PlayerType.Player2:
                return other.CompareTag("Player2");
            case PlayerType.Both:
                return other.CompareTag("Player1") || other.CompareTag("Player2");
            default:
                return false;
        }
    }
}
