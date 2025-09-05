using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
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

    [Header("ゲームオーバーの理由を表示するテキスト")]
    public Text becauseGameOverText;

    [HideInInspector]
    public static string becauseGameOver; //死因メッセを格納する変数

    SoundManager soundManager;
    SoundsList soundsList;
    PlayerCnt playerCnt;

    void Start()
    {
        //カーソルを使えるように
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //コンポーネント取得
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
        playerCnt = FindObjectOfType<PlayerCnt>();

        soundManager.OnPlaySE(soundsList.gameoverSE); //SE

        //死因を表示
        becauseGameOverText.text = "<color=red>" + "死因:" + "</color>" + becauseGameOver;

        //ヒントをランダムに表示
        RandomTips();

        //ゲームオーバーになったステージをログに送信
        if(Score.Instance.SceneName != null) OECULogging.LogInfo("ゲームオーバー:" + Score.Instance.SceneName);

    }

    //ヒントをランダムに抽出して表示
    void RandomTips()
    {
        int rnd = Random.Range(0, tips.Length);
        tipsText.text = "Tips:" + "<color=yellow>" + tips[rnd] + "</color>";
    }

    //リトライボタン
    public void OnRetryButton()
    {
        //SE
        soundManager.OnPlaySE(soundsList.clickStage);
        //ロード画面
        StartCoroutine(SceneLoading(Data.Instance.referer));
    }

    //ステージ選択へ変遷するボタン
    public void OnStageSelectButton()
    {
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
}
