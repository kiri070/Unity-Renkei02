using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_ChargeEffect : MonoBehaviour
{
    [Header("回転速度")]
    public float rotateSpeed = 30f;

    void Start()
    {
        //大きさを小さくする
        StartCoroutine(ChangeScale(new Vector3(0f, 0f, 0f), 1.5f));
    }
    void Update()
    {
        //回転　
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    //大きさを小さくする
    IEnumerator ChangeScale(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
