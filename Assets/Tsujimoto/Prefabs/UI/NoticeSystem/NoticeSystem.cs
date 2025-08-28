using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// 通知システム
/// </summary>
public class NoticeSystem : MonoBehaviour
{
    GameObject obj;
    [Header("通知パネル")] [SerializeField] GameObject panel;
    [Header("生成する親(パネル)")][SerializeField] private RectTransform parentUI;
    [Header("生成したい座標（アンカー座標）")][SerializeField] private Vector2 spawnAnchorPos;

    // プレハブ（チェックポイントUI）
    [Header("通知するオブジェクト")]
    public GameObject targetUI_CheckPoint;
    public GameObject tartgetUI_Goal;
    public GameObject tartgetUI_Timer400;
    public GameObject tartgetUI_Timer300;
    public GameObject tartgetUI_Timer200;
    public GameObject tartgetUI_Timer100;

    Image fadePanel;

    /// <summary>
    /// 通知パネルを表示します。通知するオブジェクトを代入してください
    /// </summary>
    public void ActivePanel(GameObject targetUI)
    {
        panel.SetActive(true);
        fadePanel = panel.GetComponent<Image>();
        fadePanel.DOFade(0.7f, 0.5f); //0.5秒かけて表示する
        MoveImage(targetUI);
    }

    //画像をスライドさせる
    private void MoveImage(GameObject tartgetUI)
    {
        // プレハブを親の下に生成
        obj = Instantiate(tartgetUI, parentUI);

        // RectTransformを取得
        RectTransform rt = obj.GetComponent<RectTransform>();

        // anchoredPositionを指定
        rt.anchoredPosition = spawnAnchorPos;

        // 0.5秒かけて (0,0) に移動
        rt.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic);

        StartCoroutine(NonActiveImage());
    }

    //通知システムをオフにする
    IEnumerator NonActiveImage()
    {
        yield return new WaitForSeconds(2.5f);
        fadePanel.DOKill();
        fadePanel.DOFade(0f, 1f).OnComplete(() =>
        {
            panel.SetActive(false);
        });
        Destroy(obj);
    }
}