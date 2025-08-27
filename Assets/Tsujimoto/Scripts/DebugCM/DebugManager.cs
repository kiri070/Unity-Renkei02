using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// デバックで扱うコマンド
/// </summary>
public class DebugManager : MonoBehaviour
{
    void Update()
    {
        //現在のシーンをロードする
        if (Input.GetKey(KeyCode.S))
            if (Input.GetKey(KeyCode.R))
                if (Input.GetKey(KeyCode.V))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    PlayerCnt playerCnt = FindObjectOfType<PlayerCnt>();
                    playerCnt.OnDestroyEvents(); //イベントを削除
                }
    }
}
