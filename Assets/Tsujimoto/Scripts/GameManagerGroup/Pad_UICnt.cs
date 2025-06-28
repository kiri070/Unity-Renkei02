using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Pad_UICnt : MonoBehaviour
{
    [Header("最初にフォーカスされるボタン")]
    [Tooltip("ない場合は無理に設定しなくても可")]
    [SerializeField] GameObject firstButton;

    [Header("設定画面で最初にフォーカスされるボタン")]
    [SerializeField] GameObject settingFirstButton;

    [Header("メインのUIを格納")]
    [Tooltip("※CanvasGroupが必要")]
    [SerializeField] CanvasGroup MainCanvasGroup;

    GameObject previousSelected; //変更したオブジェクトを一時格納する
    Vector3 buttonScale;         //ボタンの元のサイズ

    InputCnt pad_UICnt; //アクションマップ

    bool isControllerInputActive = false; //コントローラーが操作されたかどうか
    SettingManager settingManager;
    private void Start()
    {
        //最初にフォーカスされるボタンを設定
        // nullじゃないときだけフォーカスを当てる
        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton);

        settingManager = FindObjectOfType<SettingManager>(); // 参照取得

        pad_UICnt = new InputCnt();
        pad_UICnt.UICnt.Enable();

        //設定画面を開くボタンを登録
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
            // スティックやボタンが押されたか
            bool isInput =
                gamepad.leftStick.ReadValue().magnitude > 0.1f ||   //左スティック
                gamepad.rightStick.ReadValue().magnitude > 0.1f ||  //右スティック 
                gamepad.buttonSouth.wasPressedThisFrame ||          //Aボタン
                gamepad.buttonNorth.wasPressedThisFrame ||          //Yボタン
                gamepad.buttonEast.wasPressedThisFrame ||           //Bボタン
                gamepad.buttonWest.wasPressedThisFrame ||           //Xボタン
                gamepad.leftShoulder.wasPressedThisFrame ||         //LB
                gamepad.rightShoulder.wasPressedThisFrame ||        //RB
                gamepad.dpad.ReadValue().magnitude > 0.1f;          //Dパッド(十字キー)

            //入力されたら
            if (isInput)
            {
                isControllerInputActive = true;
            }
        }

        //フォーカスされてるボタンがなければ,フォーカスするボタンを設定
        if (isControllerInputActive && EventSystem.current.currentSelectedGameObject == null)
        {
            if (firstButton != null)
                EventSystem.current.SetSelectedGameObject(firstButton);
            isControllerInputActive = false;
        }

        //フォーカスされているボタンのサイズを変更
        Change_FocusButtonScale();
    }

    //フォーカスされているボタンのサイズを変更
    void Change_FocusButtonScale()
    {
        var selectedObj = EventSystem.current.currentSelectedGameObject; //選択中のオブジェクトを格納
        if (selectedObj == previousSelected)
            return; // 選択変わってなければ何もしない

        // 前の選択のサイズを戻す
        if (previousSelected != null)
        {
            var rtPrev = previousSelected.GetComponent<RectTransform>();
            if (rtPrev != null)
                rtPrev.localScale = buttonScale; // 元のサイズに戻す
        }

        if (selectedObj != null)
        {
            //ボタンのRectTransformを取得
            RectTransform rt = selectedObj.GetComponent<RectTransform>();
            if (rt != null)
            {
                //元のサイズを保存
                buttonScale = rt.localScale;
                //サイズを変更
                rt.localScale += new Vector3(0.5f, 0.5f, 0.5f);
            }
        }

        previousSelected = selectedObj; //変更したオブジェクトを一時格納
    }

    //設定を開く時に呼ぶ関数
    public void OpenSetting()
    {
        // メイン画面の操作を無効に
        MainCanvasGroup.interactable = false;
        MainCanvasGroup.blocksRaycasts = false;

        EventSystem.current.SetSelectedGameObject(null); // 一度フォーカスをクリア
        EventSystem.current.SetSelectedGameObject(settingFirstButton); // 設定画面のボタンを選択
    }

    //設定を閉じる時に呼ぶ関数
    public void CloseSetting()
    {
        // メイン画面の操作を有効に戻す
        MainCanvasGroup.interactable = true;
        MainCanvasGroup.blocksRaycasts = true;

        EventSystem.current.SetSelectedGameObject(null); // 一度フォーカスをクリア
        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton); // メインUIのボタンを選択
    }
}
