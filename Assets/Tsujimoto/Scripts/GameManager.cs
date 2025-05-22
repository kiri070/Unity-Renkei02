using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameState state; //ゲームの状態
    public enum GameState
    {
        Playing,
        Paused
    };

    void Start()
    {
        //初期化
        state = GameState.Playing;
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
}
