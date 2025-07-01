using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClearManager : MonoBehaviour
{
    [Header("今回のスコアを表示するテキスト")]
    public Text currentScoreText;
    float finalScore = 0;

    float[] rankings = new float[3]; //ランキングのスコアを入れる変数
    [SerializeField] Text[] rankingScoreTexts = new Text[3]; //ランキングのスコアを入れるテキスト
    void Start()
    {

        // //開発中にランキングをリセットする場合に実行
        // for (int i = 0; i < rankings.Length; i++)
        // {
        //     PlayerPrefs.DeleteKey("ScoreRank_" + i);
        //     rankings[i] = 0f;
        //     rankingScoreTexts[i].text = "0";
        // }

        //ランキング読み込み
        for (int i = 0; i < rankings.Length; i++)
        {
            rankings[i] = PlayerPrefs.GetFloat("ScoreRank_" + i, 0f); // 0f は初期値
            rankingScoreTexts[i].text = rankings[i].ToString(); //ランキングを適応
        }
        //カーソルを使えるように
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //スコア計算(残り時間 * お宝の価値)
        finalScore = Score.Instance.ScoreReferer * Score.Instance.TimeReferer;
        finalScore = (int)Mathf.Floor(finalScore);
        StartCoroutine(ShowScoreAnim(finalScore));
    }
    //リトライボタン
    public void OnRetryButton()
    {
        //前回のシーンをロード
        SceneManager.LoadScene(Data.Instance.referer);
    }

    //ステージ選択へ変遷するボタン
    public void OnStageSelectButton()
    {
        SceneManager.LoadScene("StageSelect");
    }

    //スコアにアニメーションをつける
    IEnumerator ShowScoreAnim(float finalScore)
    {
        for (int i = 0; i < 20; i++)
        {
            currentScoreText.text = Random.Range(0, finalScore + 30).ToString();
            yield return new WaitForSeconds(0.05f);
        }

        currentScoreText.text = finalScore.ToString();
        //ランキングがなにもなければ
        if (rankings == null)
        {
            rankings[0] = finalScore;
            rankingScoreTexts[0].text = rankings.ToString();
        }
        //ランキングがすでにあれば
        else
        {
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
                    rankingScoreTexts[i].text = rankings[i].ToString();
                    break;
                }
            }
        }

        //ランキングを保存
        for (int i = 0; i < rankings.Length; i++)
        {
            PlayerPrefs.SetFloat("ScoreRank_" + i, rankings[i]);
        }
    }
        
}



//クリア時にお宝の価値と残り時間を代入
public class Score
{
    public readonly static Score Instance = new Score();
    public int ScoreReferer = 0; //お宝の価値
    public float TimeReferer = 0f; //残り時間
}
