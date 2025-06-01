using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [Header("ロード画面")]
    public GameObject loadingPanel;
    public Slider loadingSlider;

    void Start()
    {
        loadingPanel.SetActive(false);
        loadingSlider.value = 0f;
    }


    //ステージ1に変遷するボタン
    public void OnStartStage1Button()
    {
        StartCoroutine(SceneLoading("GameScene01"));
    }

    //ロード画面を表示するこるーちん
    IEnumerator SceneLoading(string sceneName)
    {
        //ロード画面を表示
        loadingPanel.SetActive(true);

        //非同期読み込み
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false; //自動でシーンが切り替わらないように

        while (async.progress < 0.9f)
        {
            loadingSlider.value = async.progress; //スライダーに表示
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        async.allowSceneActivation = true; //シーン切り替え
    }
}
