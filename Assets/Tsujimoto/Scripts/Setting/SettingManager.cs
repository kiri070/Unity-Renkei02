using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("設定画面のUIを格納")]
    public GameObject settingUI;

    SoundManager soundManager;
    SoundsList soundsList;



    void Start()
    {
        //カーソルを固定して非表示
        if (SceneManager.GetActiveScene().name != "Title" && SceneManager.GetActiveScene().name != "StageSelect" &&
        SceneManager.GetActiveScene().name != "ClearScene" && SceneManager.GetActiveScene().name != "GameOver")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //コンポーネント取得
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
    }
    void Update()
    {
        OnOffSettingUI();
    }

    //設定画面を表示,非表示管理する関数
    void OnOffSettingUI()
    {
        //設定画面を表示
        if (Input.GetKeyDown(KeyCode.Escape) && !settingUI.activeSelf)
        {
            settingUI.SetActive(true); //設定画面を表示

            //効果音を再生
            soundManager.OnPlaySE(soundsList.openSetting);

            //カーソルを表示
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            //ゲームの状態をポーズに変更
            GameManager.ToPausedState();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && settingUI.activeSelf)
        {
            settingUI.SetActive(false); //設定画面を非表示

            //タイトル,ステージ選択シーン以外なら
            if (SceneManager.GetActiveScene().name != "Title" && SceneManager.GetActiveScene().name != "StageSelect")
            {
                //カーソルを非表示
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            //ゲームの状態をプレイ中に変更
            GameManager.ToPlayingState();
        }
    }

    //戻るボタン
    public void OnBackButton()
    {
        settingUI.SetActive(false); //設定画面を非表示

        //タイトル,ステージ選択シーン以外なら
        if (SceneManager.GetActiveScene().name != "Title" && SceneManager.GetActiveScene().name != "StageSelect")
        {
            //カーソルを非表示
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //ゲームの状態をプレイ中に変更
        GameManager.ToPlayingState();
    }

    //タイトルへ戻るボタン
    public void OnBackTitleButton()
    {
        //ゲームの状態をプレイ中に変更
        GameManager.ToPlayingState();
        SceneManager.LoadScene("Title");
    }

    //ステージ選択へ戻るボタン
    public void OnBackStageSelectButton()
    {
        //ゲームの状態をプレイ中に変更
        GameManager.ToPlayingState();
        SceneManager.LoadScene("StageSelect");
    }
}
