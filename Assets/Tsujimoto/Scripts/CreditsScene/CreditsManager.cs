using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// クレジットを再生する
/// </summary>

public class CreditsManager : MonoBehaviour
{
    [Header("CreditsText")][SerializeField] GameObject creditsObj;
    [Header("フェードアウトさせる画像")] Image fadeImage;
    SoundManager soundManager;
    SoundsList soundsList;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();

        fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();

        soundManager.OnPlayBGM(soundsList.tittleBGM); //BGMを再生
        StartCoroutine(MoveCredits());
    }

    void Update()
    {
        //任意のキーを押すとシーン変遷
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
        {
            OECULogging.GameEnd();
            SceneManager.LoadScene("Title");
        }
    }

    //クレジットを動かす
    IEnumerator MoveCredits()
    {
        creditsObj.SetActive(true);
        RectTransform rt = creditsObj.GetComponent<RectTransform>();
        rt.DOAnchorPosY(900f, 30f) // Y=900まで30秒で移動 //Linearで一定速度で動かす
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                StartCoroutine(EndCredits());
            });
        yield return null;
    }

    //クレジット終了後
    IEnumerator EndCredits()
    {
        //フェードアウト
        yield return new WaitForSeconds(4f);
        fadeImage.DOFade(1f, 2f).OnComplete(() =>
        {
            OECULogging.GameEnd();
            SceneManager.LoadScene("Title");
        });
            
    }
}
