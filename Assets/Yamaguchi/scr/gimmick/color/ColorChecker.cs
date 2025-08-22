using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// プレイヤーが一定範囲に入ったときに
/// ・衝突無視（PlayerごとにON/OFF可能）
/// ・半透明化（機能ON時のみ、自分自身 + 指定オブジェクト群）
/// を行うスクリプト
/// </summary>
public class ColorChecker : MonoBehaviour
{
    [Header("▼ プレイヤー検知ボックス")]
    public Vector3 boxCenterOffset = Vector3.zero;
    public Vector3 boxSize = new Vector3(3f, 3f, 3f);
    public LayerMask playerLayer;

    [Header("▼ 衝突の無効化")]
    public bool ignorePlayer1Red = true;
    public bool ignorePlayer2Blue = false;

    [Header("▼ 半透明化機能")]
    public bool enableTransparency = true;          // 半透明化機能自体のON/OFF
    public bool transparentOnPlayer1Red = true;    // Player1で透明化するか
    public bool transparentOnPlayer2Blue = true;   // Player2で透明化するか
    [Range(0f, 1f)] public float transparentAlpha = 0.3f;

    [Header("▼ 追加で半透明化するオブジェクト")]
    public GameObject[] extraTransparentObjects;

    private Collider myCollider;
    private Dictionary<Collider, bool> ignoreState = new Dictionary<Collider, bool>();

    private Renderer myRenderer;
    private Color originalColor;
    private Dictionary<GameObject, Color> extraOriginalColors = new Dictionary<GameObject, Color>();

    void Start()
    {
        myCollider = GetComponent<Collider>();
        if (myCollider == null) Debug.LogError("Collider がありません！");

        myRenderer = GetComponent<Renderer>();
        if (myRenderer != null)
        {
            originalColor = myRenderer.material.color;
            originalColor.a = 1f;
            SetMaterialToFade(myRenderer.material);
        }

        foreach (var obj in extraTransparentObjects)
        {
            if (obj == null) continue;
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                Color c = rend.material.color;
                c.a = 1f;
                extraOriginalColors[obj] = c;
                SetMaterialToFade(rend.material);
            }
        }
    }

    void Update()
    {
        Vector3 boxCenter = transform.position + boxCenterOffset;
        Collider[] players = Physics.OverlapBox(
            boxCenter,
            boxSize * 0.5f,
            Quaternion.identity,
            playerLayer
        );

        bool player1InRange = false;
        bool player2InRange = false;

        foreach (Collider col in players)
        {
            if (col == null || col == myCollider) continue;

            bool shouldIgnore = false;

            if (col.CompareTag("Player1"))
            {
                shouldIgnore = ignorePlayer1Red;
                player1InRange = true;
            }
            else if (col.CompareTag("Player2"))
            {
                shouldIgnore = ignorePlayer2Blue;
                player2InRange = true;
            }

            if (!ignoreState.ContainsKey(col) || ignoreState[col] != shouldIgnore)
            {
                Physics.IgnoreCollision(myCollider, col, shouldIgnore);
                ignoreState[col] = shouldIgnore;
            }
        }

        // 半透明化判定
        bool shouldBeTransparent = false;
        if (enableTransparency) // 機能ON時のみ判定
        {
            if (player1InRange && transparentOnPlayer1Red)
                shouldBeTransparent = true;
            else if (player2InRange && transparentOnPlayer2Blue)
                shouldBeTransparent = true;
        }

        // 自分自身の色更新
        if (myRenderer != null)
        {
            Color c = originalColor;
            c.a = shouldBeTransparent ? transparentAlpha : 1f;
            myRenderer.material.color = c;
        }

        // 追加オブジェクトの色更新
        foreach (var obj in extraTransparentObjects)
        {
            if (obj == null) continue;
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend == null) continue;

            Color baseColor = extraOriginalColors[obj];
            baseColor.a = shouldBeTransparent ? transparentAlpha : 1f;
            rend.material.color = baseColor;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.25f);
        Vector3 boxCenter = transform.position + boxCenterOffset;
        Gizmos.DrawCube(boxCenter, boxSize);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    void SetMaterialToFade(Material mat)
    {
        mat.SetFloat("_Mode", 2);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}
