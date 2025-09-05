using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SettingManager : MonoBehaviour
{
    StageSelectManager stageSelectManager;
    [Header("カウントダウンUIを格納")]
    [Tooltip("ステージシーンのみ")]public GameObject countDownUI; 
    [Header("設定画面のUIを格納")]
    public GameObject settingUI;

    SoundManager soundManager;
    SoundsList soundsList;

    PlayerCnt playerCnt;
    Pad_UICnt padUICnt;

    InputCnt inputCnt; //アクションマップ
    public bool isInputPad = false; //コントローラーを使っているかどうか
    bool isInputMouse = false; //マウスを使っているかどうか

    void Start()
    {
        //カーソルを固定して非表示
        if (SceneManager.GetActiveScene().name != "Title" && SceneManager.GetActiveScene().name != "StageSelect" &&
        SceneManager.GetActiveScene().name != "ClearScene" && SceneManager.GetActiveScene().name != "GameOver")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        inputCnt = new InputCnt();

        //コンポーネント取得
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
        playerCnt = FindObjectOfType<PlayerCnt>();
        padUICnt = FindObjectOfType<Pad_UICnt>();
        stageSelectManager = FindObjectOfType<StageSelectManager>();
    }
    void Update()
    {
        OnOffSettingUI();
        InputPadorMouse();
    }

    //コントローラーの入力を検知する関数
    public void InputPadorMouse()
    {
        var gamepad = Gamepad.current;
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        if (gamepad != null)
        {
            // スティックやボタンが押されたか
            isInputPad =
                gamepad.leftStick.ReadValue().magnitude > 0.1f ||   //左スティック
                gamepad.rightStick.ReadValue().magnitude > 0.1f ||  //右スティック 
                gamepad.buttonSouth.wasPressedThisFrame ||          //Aボタン
                gamepad.buttonNorth.wasPressedThisFrame ||          //Yボタン
                gamepad.buttonEast.wasPressedThisFrame ||           //Bボタン
                gamepad.buttonWest.wasPressedThisFrame ||           //Xボタン
                gamepad.leftShoulder.wasPressedThisFrame ||         //LB
                gamepad.rightShoulder.wasPressedThisFrame ||        //RB
                gamepad.dpad.ReadValue().magnitude > 0.1f;          //Dパッド(十字キー)
        }

        if (mouse != null && keyboard != null)
        {
            isInputMouse =
                mouse.delta.ReadValue().magnitude > 0.1f ||                  // マウス移動
                mouse.leftButton.wasPressedThisFrame ||                      // 左クリック
                mouse.rightButton.wasPressedThisFrame;                      // 右クリック
        }

        //コントローラー操作なら
        if (isInputPad)
        {
            
            if (isInputPad)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        //キーボード,マウス操作なら
        else if (isInputMouse)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //設定画面を表示,非表示管理する関数(キーボード)
    public void OnOffSettingUI()
    {
        //ステージを選択中ならバグ回避のため、設定画面を開かない
        if (stageSelectManager != null)
        {
            if (stageSelectManager.stageSelecting) return;
        }
        //カウントダウン中は設定画面を開かない
        if (countDownUI != null && countDownUI.activeSelf) return;
        
        
        //設定画面を表示
            if (Input.GetKeyDown(KeyCode.Escape) && !settingUI.activeSelf)
            {
                settingUI.SetActive(true); //設定画面を表示

                if (padUICnt != null)
                padUICnt.OpenSetting();    //設定画面のみ操作
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
                if (padUICnt != null)
                    padUICnt.CloseSetting();    //他のUIの操作を可能に
                                                //ゲームシーン以外なら
                if (SceneManager.GetActiveScene().name != "Title" && SceneManager.GetActiveScene().name != "StageSelect"
                    && SceneManager.GetActiveScene().name != "ClearScene" && SceneManager.GetActiveScene().name != "GameOverScene")
                {
                    //カーソルを非表示
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                //ゲームの状態をプレイ中に変更
                GameManager.ToPlayingState();
            }
    }

    //設定画面を表示,非表示管理する関数(コントローラー)
    public void Pad_OnOffSettingUI()
    {
        //ステージを選択中ならバグ回避のため、設定画面を開かない
        if (stageSelectManager != null)
        {
            if (stageSelectManager.stageSelecting) return;
        }
        //カウントダウン中は設定画面を開かない
        if (countDownUI != null && countDownUI.activeSelf) return;

        if (settingUI != null)
        {
            //設定画面を表示
            if (!settingUI.activeSelf)
            {
                settingUI.SetActive(true); //設定画面を表示
                padUICnt.OpenSetting();    //設定画面のみ操作
                                           //効果音を再生
                soundManager.OnPlaySE(soundsList.openSetting);

                //カーソルを表示
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                //ゲームの状態をポーズに変更
                GameManager.ToPausedState();
            }
            else if (settingUI.activeSelf)
            {
                settingUI.SetActive(false); //設定画面を非表示
                padUICnt.CloseSetting();    //他のUIの操作を可能に
                                            //ゲームシーン以外なら
                if (SceneManager.GetActiveScene().name != "Title" && SceneManager.GetActiveScene().name != "StageSelect"
                    && SceneManager.GetActiveScene().name != "ClearScene" && SceneManager.GetActiveScene().name != "GameOverScene")
                {
                    //カーソルを非表示
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                //ゲームの状態をプレイ中に変更
                GameManager.ToPlayingState();
            }
        }
    }

    //戻るボタン
    public void OnBackButton()
    {
        settingUI.SetActive(false); //設定画面を非表示
        if(padUICnt != null)
           padUICnt.CloseSetting();    //他のUIの操作を可能に
        //ゲームシーン以外なら
        if (SceneManager.GetActiveScene().name != "Title" && SceneManager.GetActiveScene().name != "StageSelect"
             && SceneManager.GetActiveScene().name != "ClearScene" && SceneManager.GetActiveScene().name != "GameOverScene")
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
        //StageSelectからTitleに戻る場合は、ゲーム終了ログ
        if (SceneManager.GetActiveScene().name == "StageSelect") OECULogging.GameEnd();

        //ゲームの状態をプレイ中に変更
        GameManager.ToPlayingState();
        SceneManager.LoadScene("Title");
    }

    //ステージ選択へ戻るボタン
    public void OnBackStageSelectButton()
    {
        //ゲームの状態をプレイ中に変更
        GameManager.ToPlayingState();
        if (playerCnt != null)
        {
            playerCnt.OnDestroyEvents(); //シーン変遷前に入力イベントを削除
        }
        SceneManager.LoadScene("StageSelect");
    }
}