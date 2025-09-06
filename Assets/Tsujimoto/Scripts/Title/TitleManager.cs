using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    [Header("点滅させるテキスト")]
    public Text keyText;
    float keyText_color; //テキストの不透明度変数
    bool flag_alpha; //不透明度を上下させるか

    [Header("フェードアウトさせる画像")]
    public Image fadeOutImage;

    bool completeFadeOut; //フェードアウトが完了したか

    SoundManager soundManager;
    SoundsList soundsList;

    [Header("デモ動画を流すまでの時間")]
    [SerializeField] float delayDemoScene;

    [Header("タイトルロゴ")] [SerializeField] Image logoImage;

    void Start()
    {
        // 例：フルHDモニターに合わせる
        Screen.SetResolution(1920, 1080, true); // 幅, 高さ, フルスクリーン

        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
        //初期化
        keyText_color = 1f;
        flag_alpha = false;
        completeFadeOut = false;

        StartCoroutine(FadeOutEffect(fadeOutImage)); //画像をフェードアウト

        soundManager.OnPlayBGM(soundsList.tittleBGM); //タイトル画面のBGMを鳴らす

        StartCoroutine(DelayDemoScene(delayDemoScene)); //指定の秒数後にデモ動画を流す

        PlayerPrefs.DeleteKey("PageIndex"); //ステージ選択画面のページ番号をリセット

        //ロゴを動かす SetEaseで動きに緩急をつける
        logoImage.transform.DOScale(1f, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
    void Update()
    {
        //任意のキーを押すとシーン変遷
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && completeFadeOut)
        {
            OECULogging.GameStart(); //ゲーム開始ログ
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

    IEnumerator DelayDemoScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("DemoMovieScene");
    }
}
