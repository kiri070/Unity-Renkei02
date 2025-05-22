using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("設定画面のUIを格納")]
    public GameObject settingUI;
    void Update()
    {
        //設定画面を表示
        if (Input.GetKey(KeyCode.Escape))
        {
            settingUI.SetActive(true); //設定画面を表示

            //カーソルを表示
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            //ゲームの状態をポーズに変更
            GameManager.ToPausedState();
        }
    }

    //戻るボタン
    public void OnBackButton()
    {
        settingUI.SetActive(false); //設定画面を非表示

        //カーソルを非表示
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //ゲームの状態をプレイ中に変更
        GameManager.ToPlayingState();
    }
}
