using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ギミックのチュートリアル動画を表示・非表示にするスクリプト
/// </summary>
public class DemoGimicMovie : MonoBehaviour
{
    [Header("チュートリアル動画")][SerializeField] GameObject movieObj;
    [Header("大きさ")][SerializeField] Vector3 maxScale = new Vector3(20f, 10f, 5);
    void Start()
    {
        movieObj.SetActive(false);
        movieObj.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        //プレイヤーが触れたら
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            ChangeActive(movieObj, true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        //プレイヤーが離れたら 
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            ChangeActive(movieObj, false);
        }
    }

    /// <summary>
    /// アクティブ状況の切り替え
    /// </summary>
    /// <param name="obj">切り替えるオブジェクト</param>
    /// <param name="active">アクティブ状況</param>
    void ChangeActive(GameObject obj, bool active)
    {
        if (active)
        {
            movieObj.transform.DOKill();
            movieObj.SetActive(true);
            movieObj.transform.DOScale(maxScale, 0.5f).SetEase(Ease.InQuad);
        }
        else if(!active)
        {
            movieObj.transform.DOKill();
            movieObj.transform.DOScale(new Vector3(0f, 0f, 0f), 0.5f).SetEase(Ease.InQuad).OnComplete(() => { obj.SetActive(active); });
        }
    }
}
