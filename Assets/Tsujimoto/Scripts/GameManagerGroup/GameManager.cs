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
