using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameState state; //ゲームの状態

    [Header("スタート時に表示するカウントダウンテキスト")]
    public Text countDownText; //最初に表示するカウントダウン
    public GameObject countDownObject; //上記の親

    [Header("ゲーム中にUI")]
    public GameObject GameUI;

    [Header("カウントダウンの秒数")]
    [SerializeField]
    private float countDown; //カウントダウンの秒数



    [Header("タイマー")]
    public float timerValue = 180f;
    [Tooltip("タイマーのテキスト")] public Text timerText;
    [SerializeField][Tooltip("タイマー減少テキスト")] GameObject decreaseTextPrefab;
    [SerializeField][Tooltip("タイマー減少テキストのスポーン位置(RectTransform)")] Transform spawnPoint;
    [Tooltip("タイム減少:ミミックの魔法を受けた時")] public float decreaseMimicTimer = 5f;
    [Tooltip("タイム減少:落下時")] public float decreaseFallTimer = 10f;

    SoundManager soundManager;
    SoundsList soundsList;
    //ゲームの状態を管理
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Clear
    };

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();


        //初期化
        ToPausedState(); //ポーズ状態

        StartCoroutine(CountDown()); //スタートまでのカウントダウン開始
    }

    void Update()
    {
        //ゲームオーバーなら
        if (state == GameState.GameOver)
        {
            SceneManager.LoadScene("GameOverScene");
        }
        //クリアなら
        else if (state == GameState.Clear)
        {
            SceneManager.LoadScene("ClearScene");
        }

        //タイマー更新
        timerValue -= Time.deltaTime;
        timerText.text = "残り時間:" + Mathf.Floor(timerValue).ToString();
        //タイマーが0になったら
        if (timerValue <= 0)
        {
            GameOverManager.becauseGameOver = "タイムアップ!";
            ToGameOverState();
        }
    }

    //タイマーを減らす関数
    public void DecreaseTimer(float time)
    {
        timerValue -= time; //タイマーを減少
        GameObject obj = Instantiate(decreaseTextPrefab, spawnPoint); //テキストを生成
        obj.transform.localScale = Vector3.one; //スケールを等倍に
        obj.transform.localPosition = Vector3.zero; //親オブジェクトの原点

        Text text = obj.GetComponent<Text>();
        if (text != null)
        {
            text.text = "-" + time.ToString();
            StartCoroutine(AnimateAndDestroyText(obj, text));
        }
    }
    //タイマー減少テキストをフェードアウトさせる
    IEnumerator AnimateAndDestroyText(GameObject obj, Text text)
    {
        float duration = 1f;
        float time = 0f;
        Vector3 startPos = obj.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 50f, 0); // 上に50動く

        Color startColor = text.color;

        while (time < duration)
        {
            float t = time / duration;

            // 上に移動
            obj.transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            // フェードアウト
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            text.color = c;

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(obj); // 最後に削除
    }

    //スタート時カウントダウン
    IEnumerator CountDown()
    {
        countDownText.text = countDown.ToString(); //テキストを表示
        while (countDown > 0)
        {
            yield return new WaitForSecondsRealtime(1f);
            countDown--;
            countDownText.text = countDown.ToString(); //テキストを表示
        }
        countDownObject.SetActive(false); //テキストを非表示
        ToPlayingState(); //プレイ開始
    }

    //ゲームモード:プレイ中に変更
    public static void ToPlayingState()
    {
        state = GameState.Playing;
        Time.timeScale = 1;
    }

    //ゲームモード:ポーズに変更
    public static void ToPausedState()
    {
        state = GameState.Paused;
        Time.timeScale = 0;
    }

    //ゲームモード:ゲームオーバーに変更
    public static void ToGameOverState()
    {
        //現在のシーン名を保存
        Data.Instance.referer = SceneManager.GetActiveScene().name;
        state = GameState.GameOver;
    }

    //ゲームモード:クリアに変更
    public static void ToClearState()
    {
        //現在のシーン名を保存
        Data.Instance.referer = SceneManager.GetActiveScene().name;
        state = GameState.Clear;
    }
}

//シーンを格納するクラス
public class Data
{
    //Dataクラスのインスタンスを作成
    public readonly static Data Instance = new Data();

    //シーン名を代入する変数
    public string referer = string.Empty;
}
