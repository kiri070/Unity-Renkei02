using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;    // Dictionary用

public class CircleArranger : MonoBehaviour
{
    [Tooltip("ステージを格納")] public Transform[] stages;
    [Tooltip("ステージの間隔")] public float radius = 5f;
    [Tooltip("ステージ全体の回転速度")] public float rotateTime = 0.4f;

    [Tooltip("ステージの選択時の大きさ(倍率)")] public float enlargeMultiplier = 1.1f;
    [Tooltip("ステージの選択中の回転速度")] public float rotateSpeed = 50f;

    private Vector3[] originalScales; // 各ステージの元サイズを保存

    int index = 0;
    float stickCooldown = 0.25f;
    float stickTimer = 0f;

    [Tooltip("設定画面のオブジェクト")] public GameObject configUI;
    [Tooltip("ステージ名を表示するテキスト")] public Text stageName;
    [Tooltip("ゲームモード選択ボタンのグループ")] public GameObject gameModeButton;
    [Tooltip("シングルプレイボタン")] public Button singlePlayButton;

    [Header("ロード画面")]
    public GameObject loadingPanel;
    public Slider loadingSlider;

    [Header("ヒントを表示するテキスト")] public Text tipsText;
    [Header("ローディング画面に表示するヒント")]
    [Tooltip("ヒントの内容を入力してください")] public string[] tips;

    SoundManager soundManager;
    SoundsList soundsList;


    //------------------------------------------------------------
    //  読み込むシーン名を変換するDictionary（ここに追加するだけで拡張できる）
    //------------------------------------------------------------
    Dictionary<string, string> sceneMap = new Dictionary<string, string>()
    {
        { "TutorialStage", "TutorialScene" },
        { "Stage1",        "Stage1Scene" },
        { "Stage2",        "Stage2Scene" },
        { "Stage3",        "Stage3Scene" },
        { "EXstage_Solo",  "Stage4Scene" },
        { "OnlyMultiStage",  "Stage5Scene" }
    };
    //------------------------------------------------------------


    void Start()
    {
        loadingPanel.SetActive(false);
        loadingSlider.value = 0f;

        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
        soundManager.OnPlayBGM(soundsList.stageSelectBGM);

        RandomTips(); //ヒント

        //ステージの初期の大きさ
        originalScales = new Vector3[stages.Length];
        for (int i = 0; i < stages.Length; i++)
            originalScales[i] = stages[i].localScale;

        //ステージを並べる
        ArrangeCircle();

        //フォーカス
        if (stages.Length > 0)
        {
            EventSystem.current.SetSelectedGameObject(stages[0].gameObject);
            UpdateStageScales();
        }
    }


    void Update()
    {

        //設定画面, ゲームモード選択時はreturn
        if ((configUI != null && configUI.activeSelf) || gameModeButton.activeSelf)
            return;

        stickTimer -= Time.deltaTime;

        ReadKeyboardInput();
        ReadGamepadInput();
        RotateSelectedStage();
        ShowStageName();
    }



    //================ 入力 ==================
    //キーボード
    void ReadKeyboardInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame ||
            Keyboard.current.dKey.wasPressedThisFrame) MoveLeft();

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame ||
            Keyboard.current.aKey.wasPressedThisFrame) MoveRight();

        if (Keyboard.current.spaceKey.wasPressedThisFrame ||
            Keyboard.current.enterKey.wasPressedThisFrame) OnSelect();
    }
    //コントローラー
    void ReadGamepadInput()
    {
        var pad = Gamepad.current;
        if (pad == null) return;

        if (pad.dpad.right.wasPressedThisFrame) MoveLeft();
        if (pad.dpad.left.wasPressedThisFrame) MoveRight();

        if (stickTimer <= 0f)
        {
            if (pad.leftStick.x.ReadValue() > 0.5f) { MoveLeft(); stickTimer = stickCooldown; }
            if (pad.leftStick.x.ReadValue() < -0.5f) { MoveRight(); stickTimer = stickCooldown; }
        }

        if (pad.buttonSouth.wasPressedThisFrame) OnSelect();
    }



    //================ 選択移動 ==================
    void MoveRight()
    {
        index = (index + 1) % stages.Length;
        EventSystem.current.SetSelectedGameObject(stages[index].gameObject);
        RotateToSelected();
        UpdateStageScales();
    }

    void MoveLeft()
    {
        index = (index - 1 + stages.Length) % stages.Length;
        EventSystem.current.SetSelectedGameObject(stages[index].gameObject);
        RotateToSelected();
        UpdateStageScales();
    }

    //決定
    void OnSelect()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        //=== BackButton が選択中なら閉じる ===
        if (current != null && current.name == "backButton")
        {
            BackButton(); // UI閉じる処理
            //ステージ選択にフォーカスを戻す
            EventSystem.current.SetSelectedGameObject(stages[index].gameObject);

            //ハイライト復活
            UpdateStageScales();
            return;
        }

        //=== UI閉じてる時はいつも通りステージ選択決定 ===
        Debug.Log("ステージ選択：" + stages[index].name);
        gameModeButton.SetActive(true);
        SelectGameMode(); // ボタンにフォーカス渡す
    }



    //================ 並び & UI反映 ==================
    void ArrangeCircle()
    {
        int count = stages.Length;

        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);

            stages[i].localPosition = pos;
            stages[i].LookAt(transform.position);
        }
    }


    void RotateToSelected()
    {
        int count = stages.Length;
        float step = 360f / count;
        float targetAngleY = -step * index;

        transform.DOKill();
        transform.DORotate(new Vector3(0, targetAngleY, 0), rotateTime).SetEase(Ease.OutCubic);
    }

    //ステージの大きさを変更する関数
    void UpdateStageScales()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            stages[i].DOKill();

            if (i == index)
            {
                Vector3 target = originalScales[i] * enlargeMultiplier;
                stages[i].DOScale(target, 0.2f).SetEase(Ease.OutQuad);
                stages[i].DOLocalMoveY(1.2f, 0.2f);
            }
            else
            {
                stages[i].DOScale(originalScales[i], 0.2f).SetEase(Ease.OutQuad);
                stages[i].DOLocalMoveY(0f, 0.2f);
            }
        }
    }

    //選択されているステージを回転させる関数
    void RotateSelectedStage()
    {
        if (stages.Length == 0) return;
        stages[index].Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    //ステージ名を表示する関数
    void ShowStageName()
    {
        stageName.text = stages[index].name;
    }

    //=================== ゲームモード制御 ===================
    void SelectGameMode()
    {
        //ボタンを取得
        GameObject single = GameObject.Find("SinglePlay");
        GameObject multi = GameObject.Find("MultiPlay");

        //ステージの名前を取得
        string name = stages[index].name;

        //===== シングル専用 =====
        if (name == "EXstage_Solo")
        {
            single.SetActive(true);
            multi.SetActive(false);

            single.GetComponent<Button>().onClick.RemoveAllListeners(); //前のイベント処理を削除
            single.GetComponent<Button>().onClick.AddListener(() => LoadSinglePlay()); //シーン変遷
            SetSelectObject(single);
        }
        //===== マルチ専用 =====
        else if (name == "OnlyMultiStage")
        {
            single.SetActive(false);
            multi.SetActive(true);

            multi.GetComponent<Button>().onClick.RemoveAllListeners(); //前のイベント処理を削除
            multi.GetComponent<Button>().onClick.AddListener(() => LoadMultiPlay()); //シーン変遷
            SetSelectObject(multi);
        }
        //===== Tutorialとその他のステージ =====
        //sceneMap辞書にnameが含まれているか
        else if (sceneMap.ContainsKey(name))
        {

            single.SetActive(true);
            multi.SetActive(true);

            string scene = sceneMap[name];

            single.GetComponent<Button>().onClick.RemoveAllListeners(); //前のイベント処理を削除
            multi.GetComponent<Button>().onClick.RemoveAllListeners();

            //シーン変遷
            single.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(SceneLoading(scene, false)));
            multi.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(SceneLoading(scene, true)));

            SetSelectObject(single);
        }
        
        
    }



    //=================== ロード入口, 戻るボタン ===================
    public void LoadSinglePlay()
    {
        string name = stages[index].name;

        if (sceneMap.ContainsKey(name)) name = sceneMap[name];

        StartCoroutine(SceneLoading(name, false));
    }

    public void LoadMultiPlay()
    {
        string name = stages[index].name;

        if (sceneMap.ContainsKey(name)) name = sceneMap[name];

        StartCoroutine(SceneLoading(name, true));
    }

    public void BackButton()
    {
        gameModeButton.SetActive(false);
    }

    //=================== ロードコルーチン ===================
    //フォーカスする関数
    void SetSelectObject(GameObject obj)
    {
        EventSystem.current.SetSelectedGameObject(obj);
    }

    //ヒントを表示する関数
    void RandomTips()
    {
        int rnd = Random.Range(0, tips.Length);
        tipsText.text = "Tips:" + "<color=yellow>" + tips[rnd] + "</color>";
    }

    IEnumerator SceneLoading(string sceneName, bool isMulti)
    {
        if (isMulti) { soundManager.OnPlaySE(soundsList.clickStage); GameManager.gameMode = GameManager.GameMode.MultiPlayer; }
        else { soundManager.OnPlaySE(soundsList.clickStage); GameManager.gameMode = GameManager.GameMode.SinglePlayer; }

        loadingPanel.SetActive(true);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            loadingSlider.value = async.progress;
            yield return null;
        }

        yield return new WaitForSeconds(0.7f);
        async.allowSceneActivation = true;
    }
}
