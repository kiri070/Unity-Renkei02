using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffectCnt : MonoBehaviour
{
    [Header("親のオブジェクト")]
    public GameObject effect_Parent;
    void Start()
    {
        StartCoroutine(ChangeScale());
    }

    IEnumerator ChangeScale()
    {
        float time = 0f;
        yield return new WaitForSeconds(0.8f);
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        while (time < 1)
        {
            float t = time / 1f;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(effect_Parent);
    }
}
