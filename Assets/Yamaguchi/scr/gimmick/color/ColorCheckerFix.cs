using UnityEngine;
using System.Collections.Generic;



public class ColorCheckerFix : MonoBehaviour
{
    [Header("▼ プレイヤー検知ボックス")]
    public Vector3 boxCenterOffset = Vector3.zero;
    public Vector3 boxSize = new Vector3(3f, 3f, 3f);
    public LayerMask playerLayer;

    [Header("▼ 衝突の無効化")]
    public bool ignorePlayer1Red = true;
    public bool ignorePlayer2Blue = false;

    [Header("▼ 半透明化機能")]
    public bool enableTransparency = true;
    public bool transparentOnPlayer1Red = true;
    public bool transparentOnPlayer2Blue = true;
    [Range(0f, 1f)] public float transparentAlpha = 0.3f;

    [Header("▼ 追加で半透明化するオブジェクト")]
    public GameObject[] extraTransparentObjects;

    private Collider myCollider;
    private Dictionary<Collider, bool> ignoreState = new Dictionary<Collider, bool>();

    private Renderer myRenderer;
    private Material[] myMaterials;
    private Color[] originalColors;

    private Dictionary<GameObject, (Material[], Color[])> extraOriginalData = new Dictionary<GameObject, (Material[], Color[])>();

    void Start()
    {
        // Collider取得
        myCollider = GetComponent<Collider>();
        if (myCollider == null) Debug.LogError("Collider がありません！");

        // 自分自身のRendererとマテリアル取得
        myRenderer = GetComponent<Renderer>();
        if (myRenderer != null)
        {
            myMaterials = myRenderer.materials; // 配列で全マテリアル取得
            originalColors = new Color[myMaterials.Length];

            for (int i = 0; i < myMaterials.Length; i++)
            {
                originalColors[i] = myMaterials[i].color;
                SetMaterialTransparent(myMaterials[i]); // 半透明モードに設定
            }
        }

        // 追加オブジェクトのRendererとマテリアル取得
        foreach (var obj in extraTransparentObjects)
        {
            if (obj == null) continue;
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                Material[] mats = rend.materials;
                Color[] cols = new Color[mats.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    cols[i] = mats[i].color;
                    SetMaterialTransparent(mats[i]);
                }
                extraOriginalData[obj] = (mats, cols);
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
            if (col.CompareTag("Player1")) { shouldIgnore = ignorePlayer1Red; player1InRange = true; }
            else if (col.CompareTag("Player2")) { shouldIgnore = ignorePlayer2Blue; player2InRange = true; }

            if (!ignoreState.ContainsKey(col) || ignoreState[col] != shouldIgnore)
            {
                Physics.IgnoreCollision(myCollider, col, shouldIgnore);
                ignoreState[col] = shouldIgnore;
            }
        }

        bool shouldBeTransparent = false;
        if (enableTransparency)
        {
            if (player1InRange && transparentOnPlayer1Red) shouldBeTransparent = true;
            if (player2InRange && transparentOnPlayer2Blue) shouldBeTransparent = true;
        }

        // 本体のマテリアル更新
        if (myRenderer != null && myMaterials != null)
        {
            for (int i = 0; i < myMaterials.Length; i++)
            {
                Color c = originalColors[i];
                c.a = shouldBeTransparent ? transparentAlpha : 1f;
                myMaterials[i].color = c;
            }
        }

        // 追加オブジェクト更新
        foreach (var kvp in extraOriginalData)
        {
            var (mats, cols) = kvp.Value;
            for (int i = 0; i < mats.Length; i++)
            {
                Color c = cols[i];
                c.a = shouldBeTransparent ? transparentAlpha : 1f;
                mats[i].color = c;
            }
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

    void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}
