using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Pad_UICnt : MonoBehaviour
{
    [Header("最初にフォーカスするボタン")]
    [Tooltip("ない場合は設定なしでも可")]
    [SerializeField] GameObject firstButton;

    [Header("設定画面で最初にフォーカスするボタン")]
    [SerializeField] GameObject settingFirstButton;

    [Header("メインUI")]
    [Tooltip("※CanvasGroupが必要")]
    [SerializeField] CanvasGroup MainCanvasGroup;

    GameObject previousSelected; //フォーカスUIを一時保存するオブジェクト
    Vector3 buttonScale;         //フォーカスUIの元のサイズを保存

    InputCnt pad_UICnt; //アクションマップ

    bool isControllerInputActive = false; //コントローラーが使われているか
    SettingManager settingManager;
    private void Start()
    {
        //最初にフォーカスするUIを設定
        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton);

        settingManager = FindObjectOfType<SettingManager>();

        pad_UICnt = new InputCnt();
        pad_UICnt.UICnt.Enable();

        //コントローラーで設定画面を開くイベントを登録
        pad_UICnt.UICnt.OpenSetting.performed += ctx =>
        {
            settingManager.Pad_OnOffSettingUI();
        };
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            //コントローラーの入力検知
            bool isInput =
                gamepad.leftStick.ReadValue().magnitude > 0.1f ||   //左スティック
                gamepad.rightStick.ReadValue().magnitude > 0.1f ||  //右スティック
                gamepad.buttonSouth.wasPressedThisFrame ||          //A
                gamepad.buttonNorth.wasPressedThisFrame ||          //Y
                gamepad.buttonEast.wasPressedThisFrame ||           //B
                gamepad.buttonWest.wasPressedThisFrame ||           //X
                gamepad.leftShoulder.wasPressedThisFrame ||         //LB
                gamepad.rightShoulder.wasPressedThisFrame ||        //RB
                gamepad.dpad.ReadValue().magnitude > 0.1f;          //DPad(十字)

            //コントローラー入力なら
            if (isInput)
            {
                isControllerInputActive = true;
            }
        }

        //UIをフォーカス
        if (isControllerInputActive && EventSystem.current.currentSelectedGameObject == null)
        {
            if (firstButton != null)
                EventSystem.current.SetSelectedGameObject(firstButton);
            isControllerInputActive = false;
        }

        //フォーカスされているUIを大きくする
        Change_FocusButtonScale();
    }

    //フォーカスされているUIを大きくする関数
    void Change_FocusButtonScale()
    {
        var selectedObj = EventSystem.current.currentSelectedGameObject; //フォーカスされているオブジェクトを格納
        if (selectedObj == previousSelected)
            return; //同じオブジェクトならスキップ

        //前回保存されたオブジェクトがあったら大きさを戻す
        if (previousSelected != null)
        {
            var rtPrev = previousSelected.GetComponent<RectTransform>();
            if (rtPrev != null)
            {
                rtPrev.localScale = buttonScale;
                //ボタンアニメーション関連
                rtPrev.DOKill(); // 途中のアニメを強制終了
                rtPrev.localScale = Vector3.one; // 初期スケールに戻す
            }
        }

        //新しいオブジェクトが選択されたら
        if (selectedObj != null)
        {
            RectTransform rt = selectedObj.GetComponent<RectTransform>();
            if (rt != null)
            {
                //大きさを保存
                buttonScale = rt.localScale;
                //大きさを変更
                rt.localScale += new Vector3(0.1f, 0.1f, 0f);

                // 選択された瞬間に軽くポップ
                rt.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1f);
            }
        }

        previousSelected = selectedObj; //新しいオブジェクトを一時保存
    }

    //設定を開いた時に呼ぶ関数
    public void OpenSetting()
    {
        //メインUIを操作できないように
        MainCanvasGroup.interactable = false;
        MainCanvasGroup.blocksRaycasts = false;

        EventSystem.current.SetSelectedGameObject(null); // 現在のフォーカスを外す
        EventSystem.current.SetSelectedGameObject(settingFirstButton); // 設定画面のUIをフォーカス
    }

    //設定を閉じた時に呼ぶ関数
    public void CloseSetting()
    {
        //メインUIを操作できるように
        MainCanvasGroup.interactable = true;
        MainCanvasGroup.blocksRaycasts = true;

        EventSystem.current.SetSelectedGameObject(null); // 現在のフォーカスを外す

        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton); // メインUIにオブジェクトがあったらフォーカス

        //現在のページのボタンをフォーカス
        StageSelectManager stageSelectManager = FindObjectOfType<StageSelectManager>();
        if (stageSelectManager != null)
            stageSelectManager.ChangePage(stageSelectManager.pageIndex);
    }
}
