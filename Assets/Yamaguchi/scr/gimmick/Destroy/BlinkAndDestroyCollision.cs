using UnityEngine;

public class BlinkAndDestroy : MonoBehaviour
{
    public float blinkDuration = 3f;   // 点滅を続ける時間
    public float blinkInterval = 0.2f; // 点滅の切り替え間隔
    private bool isBlinking = false;   // 点滅中かどうか
    public float resetTime = 3f;       // 消えた後に戻る時間

    // 点滅モード
    public enum BlinkMode { RedOnly, StepColors }
    public BlinkMode blinkMode = BlinkMode.RedOnly;

    // 元の色を保存
    private System.Collections.Generic.Dictionary<Renderer, Color> originalColors =
        new System.Collections.Generic.Dictionary<Renderer, Color>();

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

        // 最初にオリジナル色を保存
        originalColors.Clear();
        foreach (var rend in renderers)
        {
            originalColors[rend] = rend.material.color;
        }

        // 点滅ループ
        while (elapsed < blinkDuration)
        {
            // 進行度を 0〜1 に正規化
            float progress = elapsed / blinkDuration;

            // 色を決定
            Color targetColor = Color.red; // デフォルト
            if (blinkMode == BlinkMode.StepColors)
            {
                if (progress < 0.33f) targetColor = Color.green;
                else if (progress < 0.66f) targetColor = Color.yellow;
                else targetColor = Color.red;
            }

            // 点滅（ONなら指定色 / OFFなら元の色）
            foreach (var rend in renderers)
            {
                rend.material.color = visible ? targetColor : originalColors[rend];
            }

            // ON/OFF切り替え
            visible = !visible;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // 完全に消える処理
        foreach (var rend in renderers)
        {
            rend.enabled = false;
            MeshCollider col = rend.GetComponent<MeshCollider>();
            if (col != null) col.enabled = false;
        }

        // 一定時間後に戻す
        yield return new WaitForSeconds(resetTime);
        foreach (var rend in renderers)
        {
            rend.enabled = true;
            MeshCollider col = rend.GetComponent<MeshCollider>();
            if (col != null) col.enabled = true;

            // 色を元に戻す
            rend.material.color = originalColors[rend];
        }

        isBlinking = false;
    }
}
