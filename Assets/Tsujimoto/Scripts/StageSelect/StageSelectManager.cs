using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageSelectManager : MonoBehaviour
{
    public bool stageSelecting = false; //ステージを選択中かどうか
    [Header("ロード画面")]
    public GameObject loadingPanel;
    public Slider loadingSlider;

    [Header("ステージ選択後のCanvas項目")]
    [Tooltip("チュートリアルステージのprefab")][SerializeField] GameObject tutorialStagePrefab;
    [Tooltip("ステージ1のprefab")][SerializeField] GameObject stage1Prefab;
    [Tooltip("ステージの回転速度")][SerializeField] float rotateSpeed = 5f;
    [Space]

    //チュートリアルステージ
    [Header("チュートリアルステージ関連")]
    [Tooltip("チュートリアルグループ")][SerializeField] GameObject tutorial_Group;
    [Tooltip("チュートリアルステージのゲームモードボタングループ")][SerializeField] GameObject tutorial_GameMode;
    [Tooltip("チュートリアル選択後、フォーカスするボタン")][SerializeField] GameObject tutorial_firstButton;
    [Tooltip("チュートリアル選択ボタン")][SerializeField] GameObject tutorial_selectButton;
    [Space]

    //ステージ1
    [Header("ステージ1関連")]
    [Tooltip("ステージ1グループ")][SerializeField] GameObject stage1_Group;
    [Tooltip("ステージ1のゲームモードボタングループ")][SerializeField] GameObject stage1_GameMode;
    [Tooltip("ステージ1選択後、フォーカスするボタン")][SerializeField] GameObject stage1_firstButton;
    [Tooltip("ステージ1選択ボタン")][SerializeField] GameObject stage1_selectButton;


    //ヒントの内容
    [Header("ヒントを表示するテキスト")]
    public Text tipsText;
    [Header("ローディング画面に表示するヒント")]
    [Tooltip("ヒントの内容を入力してください")]
    public string[] tips;

    SoundManager soundManager;
    SoundsList soundsList;

    void Start()
    {
        //初期化
        loadingPanel.SetActive(false);
        loadingSlider.value = 0f;

        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();

        soundManager.OnPlayBGM(soundsList.stageSelectBGM); //BGM

        //ヒントをランダムに表示
        RandomTips();
    }

    void Update()
    {
        GameObject currentSelectStage = EventSystem.current.currentSelectedGameObject; //現在選択されているボタンを取得
        //チュートリアルが選択されていたら
        if (currentSelectStage.name == "Tutorial_SelectButton")
        {
            tutorialStagePrefab.transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f); //回転させる
        }
        //ステージ1が選択されていたら
        else if (currentSelectStage.name == "Stage1_SelectButton")
        {
            stage1Prefab.transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f); //回転させる
        }
    }

    //ヒントをランダムに抽出して表示
    void RandomTips()
    {
        int rnd = Random.Range(0, tips.Length);
        tipsText.text = "Tips:" + "<color=yellow>" + tips[rnd] + "</color>";
    }


    //チュートリアルに変遷するボタン
    public void OnSinglePlayTutorial()
    {
        //効果音を再生
        soundManager.OnPlaySE(soundsList.clickStage);
        //ゲームモードをシングルプレイに
        GameManager.gameMode = GameManager.GameMode.SinglePlayer;
        StartCoroutine(SceneLoading("TutorialScene"));
    }
    //チュートリアル:マルチプレイボタン
    public void OnMultiPlayTutorial()
    {
        //効果音を再生
        soundManager.OnPlaySE(soundsList.clickStage);
        //ゲームモードをマルチプレイに
        GameManager.gameMode = GameManager.GameMode.MultiPlayer;
        StartCoroutine(SceneLoading("TutorialScene"));
    }

    //チュートリアルステージを選択するボタン
    public void OnTutorialSelectButton()
    {
        //各ステージ選択ボタンを非表示
        tutorial_selectButton.SetActive(false);
        stage1_Group.SetActive(false); //一括で非表示(テキスト等含む)
        //その他のステージのprefabを非表示
        stage1Prefab.SetActive(false);
        //チュートリアルのゲームモードを決めるボタンを表示
        tutorial_GameMode.SetActive(true);
        //ボタンをフォーカスさせる
        EventSystem.current.SetSelectedGameObject(tutorial_firstButton);

        //ステージ選択中フラグを立てる
        stageSelecting = true;
    }
    //ステージ1を選択するボタン
    public void OnStage1SelectButton()
    {
        //各ステージ選択ボタンを非表示
        tutorial_Group.SetActive(false); //一括で非表示(テキスト等含む)
        stage1_selectButton.SetActive(false);
        //その他のステージのprefabを非表示
        tutorialStagePrefab.SetActive(false);
        //ステージ1のゲームモードを決めるボタンを表示
        stage1_GameMode.SetActive(true);
        //ボタンをフォーカスさせる
        EventSystem.current.SetSelectedGameObject(stage1_firstButton);

        //ステージ選択中フラグを立てる
        stageSelecting = true;
    }

    //ステージ選択に戻るボタン
    public void OnBackStageSelectButton()
    {
        SceneManager.LoadScene("StageSelect");
    }

    //ステージ1に変遷するボタン
    public void OnSinglePlayStage1()
    {
        //効果音を再生
        soundManager.OnPlaySE(soundsList.clickStage);
        //ゲームモードをシングルプレイに
        GameManager.gameMode = GameManager.GameMode.SinglePlayer;
        StartCoroutine(SceneLoading("Stage1Scene"));
    }
    //ステージ1:マルチプレイボタン
    public void OnMultiPlayStage1()
    {
        //効果音を再生
        soundManager.OnPlaySE(soundsList.clickStage);
        //ゲームモードをマルチプレイに
        GameManager.gameMode = GameManager.GameMode.MultiPlayer;
        StartCoroutine(SceneLoading("Stage1Scene"));
    }

    //ロード画面を表示するこるーちん
    IEnumerator SceneLoading(string sceneName)
    {
        //ロード画面を表示
        loadingPanel.SetActive(true);

        //非同期読み込み
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false; //自動でシーンが切り替わらないように

        while (async.progress < 0.9f)
        {
            loadingSlider.value = async.progress; //スライダーに表示
            yield return null;
        }
        yield return new WaitForSeconds(0.7f);
        async.allowSceneActivation = true; //シーン切り替え
    }
}
