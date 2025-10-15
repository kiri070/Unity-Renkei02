using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

/// <summary>
/// シングルプレイヤーのみチュートリアル
/// </summary>
public class Tutorial : MonoBehaviour
{
    [Tooltip("チュートリアルUI")] public GameObject tutorialUI;
    SettingManager settingManager;
    [Tooltip("説明テキスト")] public Text epText;
    [Tooltip("壁")] public GameObject wall1;
    [Tooltip("壁")] public GameObject wall2;
    [Tooltip("移動位置1")] public GameObject movePos1;
    [Tooltip("移動位置2")] public GameObject movePos2;

    //完了状況
    bool isLeftStick = false;
    bool isRightStick = false;

    [HideInInspector] public bool isMove_player1; //赤キャラの移動完了
    [HideInInspector] public bool isMove_player2; //青キャラの移動完了
    bool carryBox = false; //宝箱

    [HideInInspector] public bool jumpArea = false; //ジャンプエリアかどうか
    bool isjump = false; //ジャンプの完了
    void Start()
    {
        settingManager = FindObjectOfType<SettingManager>();
        settingManager.isTutorial = true;
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        //チュートリアルスキップ
        if (gamepad != null && gamepad.startButton.wasPressedThisFrame || Input.GetKeyDown(KeyCode.Tab) || GameManager.gameMode == GameManager.GameMode.MultiPlayer)
        {
            Time.timeScale = 1f;
            settingManager.isTutorial = false;
            tutorialUI.SetActive(false);
            wall1.SetActive(false);
            wall2.SetActive(false);
            movePos1.SetActive(false);
            movePos2.SetActive(false);
        }

        //コントローラー接続がなければreturn
        if (gamepad == null) return;

        
        Vector2 stick;

        // 左スティック
        if (!isLeftStick && !isMove_player1)
        {
            epText.text = "LStickで<color=red>赤</color>のキャラクターを移動";
            stick = gamepad.leftStick.ReadValue();
            if (stick.magnitude > 0.1f)
            {
                isLeftStick = true;
                Time.timeScale = 1f;
            }
        }

        // 右スティック（左が終わってから実行）
        else if (isLeftStick && !isRightStick && !isMove_player2)
        {
            epText.text = "RStickで<color=blue>青</color>のキャラクターを移動";
            stick = gamepad.rightStick.ReadValue();
            if (stick.magnitude > 0.1f)
            {
                isRightStick = true;
            }
        }

        // 両方完了したらテキストを消す
        else if (isMove_player1 && isMove_player2)
        {
            epText.text = "";
            wall1.SetActive(false);
            movePos1.SetActive(false);
        }


        //宝箱
        if (isMove_player1 && isMove_player2 && !carryBox)
        {
            movePos2.SetActive(true); //移動位置2をアクティブ
            epText.text = "[宝箱を運ぶ]\n" +
                "宝箱の前で\n" +
                "<color=red>赤</color>のキャラはL2長押し\n" +
                "<color=blue>青</color>のキャラはR2長押し";

            BringObj bringObj = FindObjectOfType<BringObj>();
            if (bringObj.player1_isBringing || bringObj.player2_isBringing) carryBox = true;
        }
        else if (carryBox)
        {
            movePos2.SetActive(false);
            epText.text = "";
        }

        //ジャンプ
        if (carryBox)
        {
            //ジャンプエリアなら
            if(jumpArea)
            {
                Time.timeScale = 0f; //時間を止める
                epText.text = "[ジャンプ]\n" +
                    "R1 or L1で敵を踏みつける";

                if (gamepad.leftShoulder.wasPressedThisFrame || gamepad.rightShoulder.wasPressedThisFrame)
                {
                    isjump = true;
                    wall1.SetActive(false);
                    Time.timeScale = 1f; //時間を進める
                }
            }


            if (isjump)
            {
                Complete(); //チュートリアル完了
            }

        }
        
    }

    //完了したら
    void Complete()
    {
        Time.timeScale = 1f;
        tutorialUI.SetActive(false);
        settingManager.isTutorial = false;
    }
}
