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

    [Header("各ステージの項目(0:チュートリアル,1:ステージ1...)")]
    [Tooltip("ステージのUIグループ")][SerializeField] private List<GameObject> stageGroups;      // UIのGroup（tutorial_Groupやstage1_Groupなど）
    [Tooltip("回転するステージのprefab")] [SerializeField] private List<GameObject> stagePrefabs;     // ステージオブジェクト（回転させるやつ）
    [Tooltip("セレクトボタン")] [SerializeField] private List<GameObject> selectButtons;    // Selectボタン
    [Tooltip("ゲームモード選択UIグループ")] [SerializeField] private List<GameObject> gameModeGroups;   // ゲームモード選択ボタンGroup
    private List<Vector3> stage_StartScale = new List<Vector3>();//ステージの初期の大きさ
    [Tooltip("ステージの回転速度")] [SerializeField] float rotateSpeed = 5f;


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

        //各ステージの初期の大きさ
        for (int i = 0; i < stagePrefabs.Count; i++)
        {
            stage_StartScale.Add(stagePrefabs[i].transform.localScale);
        }
    }

    void Update()
    {
        GameObject currentSelectStage = EventSystem.current.currentSelectedGameObject; //現在選択されているボタンを取得
        //選択されているステージの処理
        switch (currentSelectStage.name)
        {
            //チュートリアルが選択されている場合
            case "Tutorial_SelectButton":
                stagePrefabs[0].transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f); //回転させる
                stagePrefabs[0].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);   //選択中のステージを大きく
                                                                                        //選択しているステージ以外を元の大きさにする
                for (int i = 0; i < stagePrefabs.Count; i++)
                {
                    if (i != 0)
                        stagePrefabs[i].transform.localScale = stage_StartScale[i];
                }
                break;

            //ステージ1が選択されている場合
            case "Stage1_SelectButton":
                stagePrefabs[1].transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f); //回転させる
                stagePrefabs[1].transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);   //選択中のステージを大きく

                //選択しているステージ以外を元の大きさにする
                for (int i = 0; i < stagePrefabs.Count; i++)
                {
                    if (i != 1)
                        stagePrefabs[i].transform.localScale = stage_StartScale[i];
                }
                break;
        }
    }

    //ステージの切り替え関数
    void ShowStage(int index)
    {
        for (int i = 0; i < stageGroups.Count; i++)
        {
            bool isSelected = (i == index);
            stageGroups[i].SetActive(isSelected);
            stagePrefabs[i].SetActive(isSelected);
            selectButtons[i].SetActive(!isSelected);
            gameModeGroups[i].SetActive(isSelected);

            if (isSelected)
            {
                // フォーカス設定（ステージごとに設定したいなら別で配列化）
                EventSystem.current.SetSelectedGameObject(
                    gameModeGroups[i].transform.GetChild(0).gameObject //最初の子オブジェクト
                );
            }
        }

        stageSelecting = true;
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
        ShowStage(0); // tutorial
    }
    //ステージ1を選択するボタン
    public void OnStage1SelectButton()
    {
        ShowStage(1); // stage1
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
