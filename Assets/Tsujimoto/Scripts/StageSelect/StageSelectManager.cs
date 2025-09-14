using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageSelectManager : MonoBehaviour
{
    public bool stageSelecting = false; //ステージを選択中かどうか
    public int pageIndex = 0; //現在のページ
    bool nextPageButton = false; //ページを変遷したかどうか
    bool backPageButton = false; //ページを変遷したかどうか
    [Header("ロード画面")]
    public GameObject loadingPanel;
    public Slider loadingSlider;

    [Header("ステージをまとめたページ")][SerializeField] List<GameObject> page;
    [Header("ステージをまとめたobjグループ")][SerializeField] List<GameObject> stageGropuObj;
    [Header("各ステージの項目(0:チュートリアル,1:ステージ1...)")]
    [Tooltip("ステージのUIグループ")][SerializeField] private List<GameObject> stageGroups;      // UIのGroup（tutorial_Groupやstage1_Groupなど）
    [Tooltip("回転するステージのprefab")][SerializeField] private List<GameObject> stagePrefabs;     // ステージオブジェクト（回転させるやつ）
    [Tooltip("セレクトボタン")][SerializeField] private List<GameObject> selectButtons;    // Selectボタン
    [Tooltip("ゲームモード選択UIグループ")][SerializeField] private List<GameObject> gameModeGroups;   // ゲームモード選択ボタンGroup
    [Tooltip("ページ変遷ボタングループ")] [SerializeField] private List<GameObject> pageButtonGroup; 
    private List<Vector3> stage_StartScale = new List<Vector3>();//ステージの初期の大きさ
    [Tooltip("ステージの回転速度")][SerializeField] float rotateSpeed = 5f;


    //ヒントの内容
    [Header("ヒントを表示するテキスト")]
    public Text tipsText;
    [Header("ローディング画面に表示するヒント")]
    [Tooltip("ヒントの内容を入力してください")]
    public string[] tips;

    [Header("エフェクト")]
    [Tooltip("ページ変遷:左側")] public GameObject nextPageEffect_Left;
    [Tooltip("ページ変遷:右側")] public GameObject nextPageEffect_Right;

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

        //前回のページに変遷
        pageIndex = PlayerPrefs.GetInt("PageIndex", 0); // 保存されていなければ0ページ
        ChangePage(pageIndex);
    }

    void Update()
    {
        //デバックコマンド
        //ゲーム開始ログの取り消しを行う
        if(Input.GetKey(KeyCode.G))
            if(Input.GetKey(KeyCode.R))
                OECULogging.GameRevert();

        GameObject currentSelectStage = EventSystem.current.currentSelectedGameObject; //現在選択されているボタンを取得
        //選択されているステージの処理
        if (currentSelectStage != null)
        {
            switch (currentSelectStage.name)
            {
                //チュートリアルが選択されている場合
                case "Tutorial_SelectButton":
                    stagePrefabs[0].transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f); //回転させる
                    stagePrefabs[0].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);   //選択中のステージを大きく
                                                                                            //選択しているステージ以外を元の大きさにする
                    for (int i = 0; i < stagePrefabs.Count; i++)
                    {
                        if (i != 0) //このステージのindexを入れる
                            stagePrefabs[i].transform.localScale = stage_StartScale[i];
                    }
                    break;

                //ステージ1が選択されている場合
                case "Stage1_SelectButton":
                    stagePrefabs[1].transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f); //回転させる
                    stagePrefabs[1].transform.localScale = new Vector3(0.48f, 0.48f, 0.48f);   //選択中のステージを大きく

                    //選択しているステージ以外を元の大きさにする
                    for (int i = 0; i < stagePrefabs.Count; i++)
                    {
                        if (i != 1)
                            stagePrefabs[i].transform.localScale = stage_StartScale[i];
                    }
                    break;
                //ステージ2が選択されている場合
                case "Stage2_SelectButton":
                    stagePrefabs[2].transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f); //回転させる
                    stagePrefabs[2].transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);   //選択中のステージを大きく

                    //選択しているステージ以外を元の大きさにする
                    for (int i = 0; i < stagePrefabs.Count; i++)
                    {
                        if (i != 2)
                            stagePrefabs[i].transform.localScale = stage_StartScale[i];
                    }
                    break;
                //ステージ3が選択されている場合
                case "Stage3_SelectButton":
                    stagePrefabs[3].transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f); //回転させる
                    stagePrefabs[3].transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);   //選択中のステージを大きく

                    //選択しているステージ以外を元の大きさにする
                    for (int i = 0; i < stagePrefabs.Count; i++)
                    {
                        if (i != 3)
                            stagePrefabs[i].transform.localScale = stage_StartScale[i];
                    }
                    break;
                //ステージ4(EXステージ_Solo)が選択されている場合
                case "Stage4_SelectButton":
                    stagePrefabs[4].transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f); //回転させる
                    stagePrefabs[4].transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);   //選択中のステージを大きく

                    //選択しているステージ以外を元の大きさにする
                    for (int i = 0; i < stagePrefabs.Count; i++)
                    {
                        if (i != 4)
                            stagePrefabs[i].transform.localScale = stage_StartScale[i];
                    }
                    break;
                //ステージ5(マルチ専用ステージ)が選択されている場合
                case "Stage5_SelectButton":
                    stagePrefabs[5].transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f); //回転させる
                    stagePrefabs[5].transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);   //選択中のステージを大きく

                    //選択しているステージ以外を元の大きさにする
                    for (int i = 0; i < stagePrefabs.Count; i++)
                    {
                        if (i != 5)
                            stagePrefabs[i].transform.localScale = stage_StartScale[i];
                    }
                    break;
            }
        }
    }

    //次のページに行くボタン
    public void NextPageButton()
    {
        pageIndex++;
        PlayerPrefs.SetInt("PageIndex", pageIndex); // ページ番号保存
        nextPageButton = true; //次のページボタンのフラグを立てる
        ChangePage(pageIndex);
    }
    //前のページに行くボタン
    public void BackPageButton()
    {
        pageIndex--;
        PlayerPrefs.SetInt("PageIndex", pageIndex); // ページ番号保存
        backPageButton = true; //前のページボタンのフラグを立てる
        ChangePage(pageIndex);
    }

    //ページ変遷処理
    public void ChangePage(int index)
    {
        // 範囲外チェック
        if (index < 0 || index >= page.Count)
        {
            pageIndex = Mathf.Clamp(index, 0, page.Count - 1);
            index = pageIndex;
        }
        // すべて非表示
        foreach (var pageUI in page)
            pageUI.SetActive(false);
        foreach (var stageObj in stageGropuObj)
            stageObj.SetActive(false);

        // 対応ページを有効化
        page[index].SetActive(true);
        if (index < stageGropuObj.Count)
            stageGropuObj[index].SetActive(true);

        // 最初のボタンを自動フォーカス
        FocusFirstButton(page[index]);

        //次のページボタンのエフェクト
        if (nextPageButton)
        {
            GameObject nextPos = page[index].transform.Find("EffectSpawnPos_Left").gameObject;
            Instantiate(nextPageEffect_Left, nextPos.transform.position, nextPageEffect_Left.transform.rotation);
            nextPageButton = false;
        }
        //前のページボタンのエフェクト
        else if (backPageButton)
        {
            GameObject nextPos = page[index].transform.Find("EffectSpawnPos_Right").gameObject;
            Instantiate(nextPageEffect_Right, nextPos.transform.position, nextPageEffect_Right.transform.rotation);
            backPageButton = false;
        }
    }
    //ボタンをフォーカスする関数
    public void FocusFirstButton(GameObject pageObj)
    {
        Button firstButton = pageObj.GetComponentInChildren<Button>();
        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }

    //ステージの切り替え関数
    void ShowStage(int index)
    {
        //ページ変遷ボタンのグループを非表示
        foreach(GameObject ui in pageButtonGroup)
        {
            ui.SetActive(false);
        }

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

    //ステージ選択に戻るボタン
    public void OnBackStageSelectButton()
    {
        PlayerPrefs.SetInt("PageIndex", pageIndex); // ページ番号保存
        SceneManager.LoadScene("StageSelect");
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

    //各ステージに変遷するボタン(シングルプレイ)
    public void OnLoad_SingleStageButton()
    {
        // 押されたボタンを取得
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        string buttonName = clickedButton.name;

        soundManager.OnPlaySE(soundsList.clickStage);
        GameManager.gameMode = GameManager.GameMode.SinglePlayer;

        string sceneName = GetSceneName(buttonName);
        //シーン名が空でなければ
        if (!string.IsNullOrEmpty(sceneName))
            StartCoroutine(SceneLoading(sceneName)); //シーンをロード
    }

    //各ステージに変遷するボタン(マルチプレイ)
    public void OnLoad_MultiStageButton()
    {
        // 押されたボタンを取得
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        string buttonName = clickedButton.name;

        soundManager.OnPlaySE(soundsList.clickStage);
        GameManager.gameMode = GameManager.GameMode.MultiPlayer;

        string sceneName = GetSceneName(buttonName);
        //シーン名が空でなければ
        if (!string.IsNullOrEmpty(sceneName))
            StartCoroutine(SceneLoading(sceneName)); //シーンをロード
    }

    //ゲームモードボタン名からシーン名を生成する関数
    private string GetSceneName(string buttonName)
    {
        if (buttonName.StartsWith("Tutorial"))
            return "TutorialScene";

        //例:"Stage1_SinglePlay" → "Stage1Scene"
        if (buttonName.StartsWith("Stage"))
        {
            string stageNum = buttonName.Split('_')[0]; //例:_で区切り、Stage1を取り出す
            return stageNum + "Scene";
        }
        return null;
    }

    //ステージを選択した時のボタン処理
    public void StageSelectButton()
    {
        // 押されたボタンを取得
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        string buttonName = clickedButton.name;
        //チュートリアルの場合
        if (buttonName.StartsWith("Tutorial"))
        {
            ShowStage(0);
        }
        //ステージ1...の場合
        int stageNum;
        if (buttonName.StartsWith("Stage"))
        {
            string name = buttonName.Split('_')[0]; //_の前半を取得
            string num = name.Replace("Stage", ""); //Stageを空文字にする
            int.TryParse(num, out stageNum);        //文字列からint型にする
            ShowStage(stageNum);                    //ステージ選択関数を呼ぶ
        }
    }
}
