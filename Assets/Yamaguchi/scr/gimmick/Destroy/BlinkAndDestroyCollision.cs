using UnityEngine;

public class BlinkAndDestroy : MonoBehaviour
{
    public float blinkDuration = 3f;
    public float blinkInterval = 0.2f;
    private bool isBlinking = false;

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

        Destroy(gameObject);
    }
}
