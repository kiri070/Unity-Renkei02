using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

/// <summary>
/// シングル／マルチ両対応のチュートリアル管理（元の挙動を保持しつつ整理）
/// </summary>
public class Tutorial : MonoBehaviour
{
    [Header("UI / Objects")]
    [Tooltip("チュートリアルUI")] public GameObject tutorialUI;
    [Tooltip("説明テキスト")] public Text epText;

    [Tooltip("壁")] public GameObject wall1;
    public GameObject wall2;
    public GameObject wall3;
    public GameObject wall4;

    [Tooltip("移動位置")] public GameObject movePos1;
    public GameObject movePos2;

    // 状態フラグ
    bool isLeftStick = false;
    bool isRightStick = false;

    [HideInInspector] public bool isMove_player1; // 赤キャラ移動済み
    [HideInInspector] public bool isMove_player2; // 青キャラ移動済み
    bool carryBox = false;

    [HideInInspector] public bool jumpArea = false;
    bool isjump = false;
    [HideInInspector] public bool isCheckPoint = false;
    [HideInInspector] public bool isColor = false;

    // コントローラ（マルチ用）
    public Gamepad Pad1, Pad2;

    // 設定参照
    SettingManager settingManager;

    void Start()
    {
        settingManager = FindObjectOfType<SettingManager>();
        if (settingManager != null) settingManager.isTutorial = true;

        // マルチプレイ時に接続済みパッドを取得（元処理と同様）
        if (GameManager.gameMode == GameManager.GameMode.MultiPlayer)
        {
            var pads = Gamepad.all;
            if (pads.Count > 0) Pad1 = pads[0];
            if (pads.Count > 1) Pad2 = pads[1];
        }
    }

    void Update()
    {
        if (GameManager.gameMode == GameManager.GameMode.SinglePlayer)
            SinglePlayerTutorial();
        else
            MultiPlayerTutorial();
    }

    // --------------------------
    // Single Player
    // --------------------------
    void SinglePlayerTutorial()
    {
        var gamepad = Gamepad.current;

        // チュートリアルスキップ（元の優先順位・挙動を維持）
        if (gamepad != null && gamepad.startButton.wasPressedThisFrame || Input.GetKeyDown(KeyCode.Tab))
        {
            SkipTutorialAll();
            return; // スキップしたら早期returnでも問題なし
        }

        // コントローラが無ければここで抜ける（元の挙動）
        if (gamepad == null) return;

        // 左／右スティック確認
        HandleMoveSticks_Single(gamepad);

        // 宝箱（運搬）チュートリアル
        HandleCarryBox(gamepad);

        // ジャンプチュートリアル（時間停止含む）
        HandleJump_Single(gamepad);

        // チュートリアルギミック(wall3に触れて発動)
        HandleCheckpoint_Single(gamepad);

        // カラーギミック（wall4触れで発動）
        HandleColor_Single(gamepad);
    }

    void HandleMoveSticks_Single(Gamepad gamepad)
    {
        Vector2 stick;

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
        else if (isLeftStick && !isRightStick && !isMove_player2)
        {
            epText.text = "RStickで<color=blue>青</color>のキャラクターを移動";
            stick = gamepad.rightStick.ReadValue();
            if (stick.magnitude > 0.1f)
            {
                isRightStick = true;
            }
        }
        else if (isMove_player1 && isMove_player2)
        {
            epText.text = "";
            if (wall1 != null) wall1.SetActive(false);
            if (movePos1 != null) movePos1.SetActive(false);
        }
    }

    void HandleCarryBox(Gamepad gamepad)
    {
        if (isMove_player1 && isMove_player2 && !carryBox)
        {
            if (movePos2 != null) movePos2.SetActive(true);
            epText.text = "<color=yellow>[宝箱を運ぶ]</color>\n" +
                         "宝箱の前で\n" +
                         "<color=red>赤</color>のキャラはL2長押し\n" +
                         "<color=blue>青</color>のキャラはR2長押し";

            BringObj bringObj = FindObjectOfType<BringObj>();
            if (bringObj != null && (bringObj.player1_isBringing || bringObj.player2_isBringing))
            {
                carryBox = true;
            }
        }
        else if (carryBox)
        {
            if (movePos2 != null) movePos2.SetActive(false);
            epText.text = "";
        }
    }

    void HandleJump_Single(Gamepad gamepad)
    {
        if (!carryBox) return;

        if (jumpArea && !isjump)
        {
            Time.timeScale = 0f;
            epText.text = "<color=yellow>[ジャンプ]</color>\n" +
                         "R1 or L1で敵を踏みつける";

            if (gamepad.leftShoulder.wasPressedThisFrame || gamepad.rightShoulder.wasPressedThisFrame)
            {
                isjump = true;
                jumpArea = false;
                if (wall1 != null) wall1.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    void HandleCheckpoint_Single(Gamepad gamepad)
    {
        // チェックポイント表示
        if (isCheckPoint)
        {
            epText.text = "<color=yellow>[チェックポイント]</color>\n" +
                          "死亡時にここから再開できる";
            if (wall3 != null) wall3.SetActive(false);
            if (isColor) isCheckPoint = false;
        }
    }

    void HandleColor_Single(Gamepad gamepad)
    {
        if (!isColor) return;
        epText.text = "<color=yellow>[カラーギミック]</color>\n" +
                      "<color=blue>青</color>は<color=blue>青色</color>、<color=red>赤</color>は<color=red>赤色</color>とそれぞれに\n" +
                      "対応した色のみ通過できる\n" +
                      "×ボタンを押して進む";

        if (wall4 != null) wall4.SetActive(false);

        // 元の挙動どおり時間停止して入力待ち
        Time.timeScale = 0f;
        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            isColor = false;
            Time.timeScale = 1f;
            StartCoroutine(DeleteTutorial(3f));
        }
    }

    // --------------------------
    // Multi Player
    // --------------------------
    void MultiPlayerTutorial()
    {
        // チュートリアルスキップ（元の優先順位を保持）
        if (Pad1.startButton.wasPressedThisFrame || Pad2.startButton.wasPressedThisFrame || Input.GetKeyDown(KeyCode.Tab))
        {
            SkipTutorialAll();
            return;
        }

        // 元コードと同様、Pad1/Pad2がnullならreturn
        if (Pad1 == null || Pad2 == null) return;

        HandleMoveSticks_Multi();
        HandleCarryBox_Multi();
        HandleJump_Multi();
        HandleCheckpoint_Multi();
        HandleColor_Multi();
    }

    void HandleMoveSticks_Multi()
    {
        Vector2 stick;

        if (!isLeftStick && !isMove_player1)
        {
            epText.text = "プレイヤー1は\nLStickで<color=red>赤</color>のキャラクターを移動";
            stick = Pad1.leftStick.ReadValue();
            if (stick.magnitude > 0.1f)
            {
                isLeftStick = true;
                Time.timeScale = 1f;
            }
        }
        else if (isLeftStick && !isRightStick && !isMove_player2)
        {
            epText.text = "プレイヤー2は\nLStickで<color=blue>青</color>のキャラクターを移動";
            stick = Pad2.leftStick.ReadValue();
            if (stick.magnitude > 0.1f)
            {
                isRightStick = true;
            }
        }
        else if (isMove_player1 && isMove_player2)
        {
            epText.text = "";
            if (wall1 != null) wall1.SetActive(false);
            if (movePos1 != null) movePos1.SetActive(false);
        }
    }

    void HandleCarryBox_Multi()
    {
        if (isMove_player1 && isMove_player2 && !carryBox)
        {
            if (movePos2 != null) movePos2.SetActive(true);
            epText.text = "<color=yellow>[宝箱を運ぶ]</color>\n" +
                         "宝箱の前で\n" +
                         "R2 または L2を長押し";

            BringObj bringObj = FindObjectOfType<BringObj>();
            if (bringObj != null && (bringObj.player1_isBringing || bringObj.player2_isBringing))
            {
                carryBox = true;
            }
        }
        else if (carryBox)
        {
            if (movePos2 != null) movePos2.SetActive(false);
            epText.text = "";
        }
    }

    void HandleJump_Multi()
    {
        if (!carryBox) return;

        if (jumpArea && !isjump)
        {
            Time.timeScale = 0f;
            epText.text = "<color=yellow>[ジャンプ]</color>\n" +
                         "R1 or L1で敵を踏みつける";

            if (Pad1.leftShoulder.wasPressedThisFrame || Pad1.rightShoulder.wasPressedThisFrame ||
                Pad2.leftShoulder.wasPressedThisFrame || Pad2.rightShoulder.wasPressedThisFrame)
            {
                isjump = true;
                jumpArea = false;
                if (wall1 != null) wall1.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    void HandleCheckpoint_Multi()
    {
        if (isjump && isCheckPoint)
        {
            epText.text = "<color=yellow>[チェックポイント]</color>\n" +
                          "死亡時にここから再開できる";
            if (wall3 != null) wall3.SetActive(false);
            if (isColor) isCheckPoint = false;

        }
    }

    void HandleColor_Multi()
    {
        if (!isColor) return;

        epText.text = "<color=yellow>[カラーギミック]</color>\n" +
                      "<color=blue>青</color>は<color=blue>青色</color>、<color=red>赤</color>は<color=red>赤色</color>とそれぞれに\n" +
                      "対応した色のみ通過できる\n" +
                      "×ボタンを押して進む";

        // 元の挙動どおり時間停止して入力待ち
        Time.timeScale = 0f;
        if (Pad1.buttonSouth.wasPressedThisFrame || Pad2.buttonSouth.wasPressedThisFrame)
        {
            isColor = false;
            Time.timeScale = 1f;
            StartCoroutine(DeleteTutorial(3f));
        }
    }

    // --------------------------
    // 共通ユーティリティ
    // --------------------------
    void SkipTutorialAll()
    {
        if (settingManager != null) settingManager.isTutorial = false;
        if (tutorialUI != null) tutorialUI.SetActive(false);

        if (wall1 != null) wall1.SetActive(false);
        if (wall2 != null) wall2.SetActive(false);
        if (wall3 != null) wall3.SetActive(false);
        if (wall4 != null) wall4.SetActive(false);

        if (movePos1 != null) movePos1.SetActive(false);
        if (movePos2 != null) movePos2.SetActive(false);

        jumpArea = false;
        Time.timeScale = 1f;
    }

    IEnumerator DeleteTutorial(float time)
    {
        yield return new WaitForSeconds(time);
        Complete_Tutorial();
    }

    void Complete_Tutorial()
    {
        Time.timeScale = 1f;
        if (tutorialUI != null) tutorialUI.SetActive(false);
        if (settingManager != null) settingManager.isTutorial = false;
    }
}
