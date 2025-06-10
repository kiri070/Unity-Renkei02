using UnityEngine;

public class EnemyToggle : MonoBehaviour
{
    [Tooltip("この敵が初期状態で表示されるかどうか")]
    public bool isOnAtStart = true; // 初期の表示状態（Inspectorから設定可能）

    private bool isOn; // 現在の表示状態（内部的に管理）

    void Start()
    {
        // 初期状態での表示/非表示を設定
        isOn = isOnAtStart;
        gameObject.SetActive(isOn);
    }

    // スイッチから呼び出され、表示状態を反転する
    public void Toggle()
    {
        isOn = !isOn;
        gameObject.SetActive(isOn); // 表示・非表示を切り替え
        Debug.Log($"{gameObject.name} の表示状態: {isOn}");
    }
}

