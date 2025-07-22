using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_ChargeEffect : MonoBehaviour
{
    [Header("�`���[�W�G�t�F�N�g�̉�]���x")]
    public float rotateSpeed = 30f;

    void Start()
    {
        //�傫��������������R���[�`��
        StartCoroutine(ChangeScale(new Vector3(0f, 0f, 0f), 1.5f));
    }
    void Update()
    {
        //��]
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    //�傫�������������Ă���
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
