using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ClearManager : MonoBehaviour
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

    [Header("今回のスコアを表示するテキスト")]
    public Text currentScoreText;
    float finalScore = 0;

    //テキストの強調表示
    float keyText_color = 1f; //透明度の初期値
    bool flag_alpha = false; //透明にするかどうか

    float[] rankings = new float[3]; //ランキングのスコアを入れる変数
    [SerializeField] Text[] rankingScoreTexts = new Text[3]; //ランキングのスコアを入れるテキスト

    [Header("メインUIを格納")][SerializeField] CanvasGroup mainUI;

    SoundManager soundManager;
    SoundsList soundsList;

    [SerializeField]GameObject deleteScoreUI; //スコアランキングを削除した時に表示するUI


    //スコアを消す関数
    void ClearScoreOnly()
    {
        string keyPrefix = "ScoreRank_" + Score.Instance.SceneName + "_";
        for (int i = 0; i < rankings.Length; i++)
        {
            PlayerPrefs.DeleteKey(keyPrefix + i);
            rankings[i] = 0f;
            rankingScoreTexts[i].text = "0";
        }
        PlayerPrefs.Save(); // 変更を反映
    }
    void Start()
    {
        // //開発中にランキングをリセットする場合に実行
        // for (int i = 0; i < rankings.Length; i++)
        // {
        //     PlayerPrefs.DeleteAll();
        //     rankings[i] = 0f;
        //     rankingScoreTexts[i].text = "0";
        //     PlayerPrefs.Save();
        // }


        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();

        //SE
        soundManager.OnPlaySE(soundsList.gameclearSE);

        //一時UIを操作不可に
        mainUI.interactable = false;
        mainUI.blocksRaycasts = false;

        string keyPrefix = "ScoreRank_" + Score.Instance.SceneName + "_";
        for (int i = 0; i < rankings.Length; i++)
        {
            rankings[i] = PlayerPrefs.GetFloat(keyPrefix + i, 0f); // シーンごとのキーで読み込む
            rankingScoreTexts[i].text = rankings[i].ToString();
        }

        //カーソルを使えるように
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //スコア計算(残り時間 * お宝の価値)
        finalScore = Score.Instance.ScoreReferer * Score.Instance.TimeReferer;
        finalScore = (int)Mathf.Floor(finalScore);
        StartCoroutine(ShowScoreAnim(finalScore));

        //ヒントをランダムに表示
        RandomTips();

        //ステージをクリアした場合、クリアステージをログに送信
        if (Score.Instance.SceneName != null) OECULogging.LogInfo("クリア:" + Score.Instance.SceneName);
    }

    void Update()
    {
        //スコアランキングを消す
        if (Input.GetKey(KeyCode.S))
            if (Input.GetKey(KeyCode.R))
                if (Input.GetKey(KeyCode.C))
                {
                    ClearScoreOnly(); //スコアランキングを削除
                    deleteScoreUI.SetActive(true); //削除通知を表示
                    StartCoroutine(DelayNonActive_DeleteScoreUI()); //遅延をかけて削除通知を非表示
                }
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
        //前回のシーンをロード
        StartCoroutine(SceneLoading(Data.Instance.referer));
        soundManager.OnPlaySE(soundsList.clickStage); //SE
    }

    //ステージ選択へ変遷するボタン
    public void OnStageSelectButton()
    {
        SceneManager.LoadScene("StageSelect");
    }

    //クレジットを表示するボタン
    public void OnCreditsButton()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    //スコアにアニメーションをつける
    IEnumerator ShowScoreAnim(float finalScore)
    {
        //ランダムな数字を表示する
        for (int i = 0; i < 20; i++)
        {
            currentScoreText.text = Random.Range(0, finalScore + 30).ToString();
            yield return new WaitForSeconds(0.05f);
        }
        //今回のスコアを表示
        currentScoreText.text = finalScore.ToString();
        //ランキングがすでにあれば
        for (int i = 0; i < rankings.Length; i++)
        {
            if (finalScore >= rankings[i])
            {
                // ランキングを下にずらす（末尾から）
                for (int j = rankings.Length - 1; j > i; j--)
                {
                    rankings[j] = rankings[j - 1];
                    rankingScoreTexts[j].text = rankings[j].ToString();
                }

                // 新しいスコアを挿入
                rankings[i] = finalScore;
                rankingScoreTexts[i].text = rankings[i].ToString(); // テキストにスコアを入れる
                rankingScoreTexts[i].color = new Color(1f, 1f, 0f, 1f); // 黄色（透明度つき）
                StartCoroutine(TextAnim(rankingScoreTexts[i])); //テキストアニメーションを開始
                break;
            }
        }

        //保存
        string keyPrefix = "ScoreRank_" + Score.Instance.SceneName + "_";
        for (int i = 0; i < rankings.Length; i++)
        {
            PlayerPrefs.SetFloat(keyPrefix + i, rankings[i]); // シーンごとのキーで保存
        }

        //UIを操作可能にする
        mainUI.interactable = true;
        mainUI.blocksRaycasts = true;
    }

    //自分のスコアがランキングに入ったら強調表示
    IEnumerator TextAnim(Text text)
    {
        while (true)
        {
            //表示させる
            if (flag_alpha)
            {
                keyText_color += Time.deltaTime;
                text.color = new Color(Color.black.r, Color.black.g, Color.black.b, keyText_color);
            }
            //透明にする
            else if (!flag_alpha)
            {
                keyText_color -= Time.deltaTime;
                text.color = new Color(Color.black.r, Color.black.g, Color.black.b, keyText_color);
            }

            //透明になったら
            if (keyText_color <= 0)
            {
                flag_alpha = true;
            }
            //透明じゃないなら
            else if (keyText_color >= 1)
            {
                flag_alpha = false;
            }
            yield return null;
        }
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

    //スコア削除UIを非表示
    IEnumerator DelayNonActive_DeleteScoreUI()
    {
        yield return new WaitForSeconds(1);
        deleteScoreUI.SetActive(false);
    }

}
//クリア時にお宝の価値と残り時間を代入
public class Score
{
    public readonly static Score Instance = new Score();
    public int ScoreReferer = 0; //お宝の価値
    public float TimeReferer = 0f; //残り時間
    public string SceneName = string.Empty; //シーン名
}
