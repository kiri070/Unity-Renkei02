using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameState state; //ゲームの状態
    public static GameMode gameMode; //ゲームモード
    PlayerCnt playerCnt;

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

    [Header("お宝関連")]
    [Tooltip("価値を表示するテキスト")]public Text valueText;
    [HideInInspector] public int boxValue = 100; //お宝の価値
    [SerializeField][Tooltip("価値の加減を表示するテキスト")] GameObject valueTextPrefab;
    [SerializeField][Tooltip("価値の加減を表示するテキストのスポーン位置")] Transform valueTextPrefab_SpawnPoint;


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

        //お宝の価値を表示
        valueText.text = "<color=yellow>" + "お宝の価値:" + boxValue.ToString() + "%" + "</color>";

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
            SceneManager.LoadScene("GameOverScene");
        }
        //クリアなら
        else if (state == GameState.Clear)
        {
            playerCnt.OnDestroyEvents(); //シーン変遷前に入力イベントを削除
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

    //お宝の価値を下げる関数
    public void MinusBoxValue(int value)
    {
        boxValue -= value;
        if (boxValue <= 0) boxValue = 0;
        //お宝の価値を更新
        if (boxValue >= 80) //80以上なら文字色:黄色
            valueText.text = "<color=yellow>" + "お宝の価値:" + boxValue.ToString() + "%" + "</color>";
        else if (boxValue < 80 && boxValue >= 50) //80未満,50以上なら文字色:青色
            valueText.text = "<color=yellow>" + "お宝の価値:" + "<color=blue>" + boxValue.ToString() + "%" + "</color>" + "</color>";
        else if (boxValue < 50 && boxValue >= 10) //50未満,10以上なら文字色:赤色
            valueText.text = "<color=yellow>" + "お宝の価値:" + "<color=red>" + boxValue.ToString() + "%" + "</color>" + "</color>";
        else if (boxValue < 10) //10未満なら茶色
            valueText.text = "<color=yellow>" + "お宝の価値:" + "<color=brown>" + boxValue.ToString() + "%" + "</color>" + "</color>";

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
