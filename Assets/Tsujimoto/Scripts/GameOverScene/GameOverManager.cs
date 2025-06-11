using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("ゲームオーバーの理由を表示するテキスト")]
    public Text becauseGameOverText;

    [HideInInspector]
    public static string becauseGameOver; //死因メッセを格納する変数

    SoundManager soundManager;
    SoundsList soundsList;
    void Start()
    {
        //カーソルを使えるように
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //コンポーネント取得
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();

        //死因を表示
        becauseGameOverText.text = "<color=red>" + "死因:" + "</color>" + becauseGameOver;
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
