using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("点滅させるテキスト")]
    public Text keyText;
    float keyText_color; //テキストの不透明度変数
    bool flag_alpha; //不透明度を上下させるか

    [Header("フェードアウトさせる画像")]
    public Image fadeOutImage;

    bool completeFadeOut; //フェードアウトが完了したか

    void Start()
    {
        //初期化
        keyText_color = 1f;     
        flag_alpha = false;     
        completeFadeOut = false;

        StartCoroutine(FadeOutEffect(fadeOutImage)); //画像をフェードアウト
    }
    void Update()
    {
        //任意のキーを押すとシーン変遷
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && completeFadeOut)
        {
            SceneManager.LoadScene("StageSelect");
        }

        ChangeTextAlpha();
    }

    //テキストの点滅をする関数
    void ChangeTextAlpha()
    {
        keyText.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, keyText_color);

        keyText_color -= Time.deltaTime * 0.1f;

        //表示させる
        if (flag_alpha)
        {
            keyText_color += Time.deltaTime;
        }
        //透明にする
        else if (!flag_alpha)
        {
            keyText_color -= Time.deltaTime;
        }

        //透明になったら
        if (keyText_color <= 0)
        {
            flag_alpha = true;
        }
        //透明じゃないなら
        else if (keyText_color >= 1)
        {
            flag_alpha = false;
        }
    }

    //フェードアウト処理
    IEnumerator FadeOutEffect(Image image)
    {
        float alpha = 1f;
        while (alpha > 0)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            alpha -= Time.deltaTime;
            yield return null;
        }
        completeFadeOut = true;
    }
}
