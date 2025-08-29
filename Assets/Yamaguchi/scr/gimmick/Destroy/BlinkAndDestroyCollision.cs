using UnityEngine;

public class BlinkAndDestroy : MonoBehaviour
{
    public float blinkDuration = 3f;
    public float blinkInterval = 0.2f;
    private bool isBlinking = false;
    public float resetTime = 3f;

    public void StartBlinking()
    {
        if (!isBlinking)
        {
            StartCoroutine(BlinkThenDestroy());
        }
    }

    private System.Collections.IEnumerator BlinkThenDestroy()
    {
        isBlinking = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < blinkDuration)
        {
            foreach (var rend in renderers)
            {
                rend.enabled = visible;
            }

            visible = !visible;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        //renderer, colliderをオフ
        foreach (var rend in renderers)
        {
            rend.enabled = false;
            rend.GetComponent<MeshCollider>().enabled = false;
        }

        //renderer, colliderを戻す
        yield return new WaitForSeconds(resetTime);
        foreach (var rend in renderers)
        {
            rend.enabled = true;
            rend.GetComponent<MeshCollider>().enabled = true;
            isBlinking = false;
        }
        // Destroy(gameObject);
    }
}
