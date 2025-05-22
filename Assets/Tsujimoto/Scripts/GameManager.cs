using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        Paused
    };

    //ゲームモード:プレイ中に変更
    public void ToPlayingState(GameState state)
    {
        state = GameState.Playing;
    }

    //ゲームモード:ポーズに変更
    public void ToPausedState(GameState state)
    {
        state = GameState.Paused;
    }
}
