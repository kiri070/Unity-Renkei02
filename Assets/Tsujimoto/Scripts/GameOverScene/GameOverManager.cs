using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{

    void Start()
    {
        //カーソルを使えるように
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
}
