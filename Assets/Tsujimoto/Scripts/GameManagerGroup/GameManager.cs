using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public static GameState state; //ゲームの状態
    public static GameMode gameMode; //ゲームモード
    PlayerCnt playerCnt;

    [Header("スタート時に表示するカウントダウンテキスト")]
    public Text countDownText; //最初に表示するカウントダウン
    public GameObject countDownObject; //上記の親

    [Header("ゲーム中のUI")]
    public GameObject GameUI;
    [Tooltip("残り時間を知らせるUI")]public GameObject timeTextGroup;
    [Tooltip("残り時間をテキスト")] public Text leftoverTimeText;

    [Header("カウントダウンの秒数")]
    [SerializeField]
    private float countDown; //カウントダウンの秒数



    [Header("タイマー")]
    public float timerValue = 180f;

    public bool timerActive = true; //タイマーを作動させるかどうか
    [Tooltip("タイマーのテキスト")] public Text timerText;
    [SerializeField][Tooltip("タイマー減少テキスト")] GameObject decreaseTextPrefab;
    [SerializeField][Tooltip("タイマー減少テキストのスポーン位置(RectTransform)")] Transform spawnPoint;
    [Tooltip("タイム減少:ミミックの魔法を受けた時")] public float decreaseMimicTimer = 5f;
    [Tooltip("タイム減少:落下時")] public float decreaseFallTimer = 10f;

    [Header("お宝関連")]
    [Tooltip("価値を表示するテキスト")]public Text valueText;
    [HideInInspector] public int boxValue = 100; //お宝の価値
    [SerializeField][Tooltip("価値の加減を表示するテキスト")] GameObject valueTextPrefab;
    [SerializeField][Tooltip("価値の加減を表示するテキストのスポーン位置")] Transform valueTextPrefab_SpawnPoint;


    SoundManager soundManager;
    SoundsList soundsList;

    TimerNeedle timerNeedle;
    NoticeSystem noticeSystem;

    //残り時間の計測
    bool time400, time300, time200, time100 = false;

    Image redPanel; //残り時間が少なくなると点滅するパネル
    bool aleardyRedPanel = false;

    
    //ゲームの状態を管理
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Clear
    };

    //ゲームモード
    public enum GameMode
    {
        SinglePlayer,
        MultiPlayer
    };

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
        playerCnt = FindObjectOfType<PlayerCnt>();
        timerNeedle = FindObjectOfType<TimerNeedle>();
        noticeSystem = FindObjectOfType<NoticeSystem>();

        redPanel = GameObject.Find("RedPanel").GetComponent<Image>();

        //お宝の価値を表示
        //valueText.text = "<color=yellow>" + "お宝の価値:" + boxValue.ToString() + "%" + "</color>";
        valueText.text = boxValue.ToString() + "%";

        //初期化
        ToPausedState(); //ポーズ状態

        StartCoroutine(CountDown()); //スタートまでのカウントダウン開始
    }

    void Update()
    {
        //ゲームオーバーなら
        if (state == GameState.GameOver)
        {
            playerCnt.OnDestroyEvents(); //シーン変遷前に入力イベントを削除
            Score.Instance.SceneName = SceneManager.GetActiveScene().name; //シーン名を代入
            SceneManager.LoadScene("GameOverScene");
        }
        //クリアなら
        else if (state == GameState.Clear)
        {
            Score.Instance.ScoreReferer = boxValue; //お宝の価値を代入
            Score.Instance.TimeReferer = timerValue; //残り時間を代入
            Score.Instance.SceneName = SceneManager.GetActiveScene().name; //シーン名を代入
            playerCnt.OnDestroyEvents(); //シーン変遷前に入力イベントを削除
            SceneManager.LoadScene("ClearScene");
        }

        Timer();
        ShowTime();

        //時間が少なくなったら画面を点滅させる
        if (timerValue <= 50 && !aleardyRedPanel) FlashRed();
    }

    //画面を赤く点滅させる
    void FlashRed()
    {
        aleardyRedPanel = true;
        redPanel.DOFade(0.3f, 0.5f).SetLoops(6, LoopType.Yoyo);
    }

    //タイマー
    void Timer()
    {
        //タイマー更新
        if (timerActive)
        {
            timerValue -= Time.deltaTime;
            timerText.text = "残り時間:" + Mathf.Floor(timerValue).ToString();
            //CheckTimerUI();
        }
        //タイマーが0になったら
        if (timerValue <= 0)
        {
            GameOverManager.becauseGameOver = "タイムアップ!";
            ToGameOverState();
        }
    }

    //残り時間を表示する関数
    void ShowTime()
    {
        //400秒以下
        if (timerValue <= 400f && !time400)
        {
            noticeSystem.ActivePanel(noticeSystem.tartgetUI_Timer400);
            time400 = true;
        }
        else if (timerValue <= 300f && !time300)
        {
            noticeSystem.ActivePanel(noticeSystem.tartgetUI_Timer300);
            time300 = true;
        }
        else if (timerValue <= 200f && !time200)
        {
            noticeSystem.ActivePanel(noticeSystem.tartgetUI_Timer200);
            time200 = true;
        }
        else if (timerValue <= 100f && !time100)
        {
            noticeSystem.ActivePanel(noticeSystem.tartgetUI_Timer100);
            time100 = true;
        }
    }

    //お宝の価値を下げる関数
    public void MinusBoxValue(int value)
    {
        boxValue -= value;
        if (boxValue <= 0) boxValue = 0;
        //お宝の価値を更新
        //if (boxValue >= 80) //80以上なら文字色:黄色
        //    valueText.text = "<color=yellow>" + "お宝の価値:" + boxValue.ToString() + "%" + "</color>";
        //else if (boxValue < 80 && boxValue >= 50) //80未満,50以上なら文字色:青色
        //    valueText.text = "<color=yellow>" + "お宝の価値:" + "<color=blue>" + boxValue.ToString() + "%" + "</color>" + "</color>";
        //else if (boxValue < 50 && boxValue >= 10) //50未満,10以上なら文字色:赤色
        //    valueText.text = "<color=yellow>" + "お宝の価値:" + "<color=red>" + boxValue.ToString() + "%" + "</color>" + "</color>";
        //else if (boxValue < 10) //10未満なら茶色
        //    valueText.text = "<color=yellow>" + "お宝の価値:" + "<color=brown>" + boxValue.ToString() + "%" + "</color>" + "</color>";
        if (boxValue >= 80) //80以上なら文字色:黄色
            valueText.text = boxValue.ToString() + "%";
        else if (boxValue < 80 && boxValue >= 50) //80未満,50以上なら文字色:青色
            valueText.text = boxValue.ToString() + "%";
        else if (boxValue < 50 && boxValue >= 10) //50未満,10以上なら文字色:赤色
            valueText.text = boxValue.ToString() + "%";
        else if (boxValue < 10) //10未満なら茶色
            valueText.text = boxValue.ToString() + "%";

        // マイナステキストを表示
        GameObject minusObj = Instantiate(valueTextPrefab, valueTextPrefab_SpawnPoint);
        minusObj.transform.localScale = Vector3.one;
        minusObj.transform.localPosition = Vector3.zero;

        Text minusText = minusObj.GetComponent<Text>();
        if (minusText != null)
        {
            minusText.text = "<color=red>" + "-" + value.ToString() + "%" + "</color>";
            StartCoroutine(AnimateAndDestroyText(minusObj, minusText));
        }

        valueText.transform.DOShakePosition(0.5f, new Vector3(3, 3), 50); //お宝の価値のテキストを揺らす

    }
    //お宝の価値を回復する関数
    public void PlusBoxValue(int value)
    {
        boxValue += value;
        //お宝の価値を更新
        if (boxValue >= 80) //80以上なら文字色:黄色
            valueText.text = "<color=yellow>" + "お宝の価値:" + boxValue.ToString() + "%" + "</color>";
        else if (boxValue < 80 && boxValue >= 50) //80未満,50以上なら文字色:青色
            valueText.text = "<color=yellow>" + "お宝の価値:" + "<color=blue>" + boxValue.ToString() + "%" + "</color>" + "</color>";
        else if (boxValue < 50 && boxValue >= 10) //50未満,10以上なら文字色:赤色
            valueText.text = "<color=yellow>" + "お宝の価値:" + "<color=red>" + boxValue.ToString() + "%" + "</color>" + "</color>";
        else if (boxValue < 10) //10未満なら茶色
            valueText.text = "<color=yellow>" + "お宝の価値:" + "<color=brown>" + boxValue.ToString() + "%" + "</color>" + "</color>";

        // プラステキストを表示
        GameObject plusObj = Instantiate(valueTextPrefab, valueTextPrefab_SpawnPoint);
        plusObj.transform.localScale = Vector3.one;
        plusObj.transform.localPosition = Vector3.zero;

        Text plusText = plusObj.GetComponent<Text>();
        if (plusText != null)
        {
            plusText.text = "<color=yellow>" + "+" + value.ToString() + "%" + "</color>";
            StartCoroutine(AnimateAndDestroyText(plusObj, plusText));
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

        timerText.transform.DOShakePosition(0.5f, new Vector3(3, 3), 50); //残り時間のテキストを揺らす

        timerNeedle.DecreaseTimerNeedle(); //タイマーの針を動かす
    }
    //テキストをフェードアウトさせる
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

    /// <summary>
    /// ゲームモードをPlayingに変更します。
    /// </summary>
    public static void ToPlayingState()
    {
        state = GameState.Playing;
        Time.timeScale = 1;
    }

    /// <summary>
    /// ゲームモードをPausedに変更します。
    /// </summary>
    public static void ToPausedState()
    {
        state = GameState.Paused;
        Time.timeScale = 0;
    }

    /// <summary>
    /// ゲームモードをGameOverに変更します。
    /// </summary>
    public static void ToGameOverState()
    {
        //現在のシーン名を保存
        Data.Instance.referer = SceneManager.GetActiveScene().name;
        state = GameState.GameOver;
    }

    /// <summary>
    /// ゲームモードをClearに変更します。
    /// </summary>
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
