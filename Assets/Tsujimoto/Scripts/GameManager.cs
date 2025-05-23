using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameState state; //ゲームの状態
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Clear
    };

    void Start()
    {
        //初期化
        state = GameState.Playing;
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
        state = GameState.GameOver;
    }

    //ゲームモード:クリアに変更
    public static void ToClearState()
    {
        state = GameState.Clear;
    }
}
