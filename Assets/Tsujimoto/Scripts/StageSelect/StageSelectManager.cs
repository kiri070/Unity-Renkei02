using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageSelectManager : MonoBehaviour
{
    [Header("ロード画面")]
    public GameObject loadingPanel;
    public Slider loadingSlider;


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
