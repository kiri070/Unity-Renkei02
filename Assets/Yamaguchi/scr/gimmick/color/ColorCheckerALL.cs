using UnityEngine;
using System.Collections.Generic;

public class ColorCheckerALL : MonoBehaviour
{
    [Header("▼ プレイヤー検知ボックス")]
    public Vector3 boxCenterOffset = Vector3.zero;
    public Vector3 boxSize = new Vector3(3f, 3f, 3f);
    public LayerMask playerLayer;

    [Header("▼ 衝突の無効化（タグ指定）")]
    [Tooltip("このタグを持つオブジェクトと衝突しないようにする")]
    public string[] ignoreCollisionTags;

    [Header("▼ 半透明化機能")]
    public bool enableTransparency = true;          // 半透明化機能自体のON/OFF
    public string[] transparentOnTags;              // このタグのオブジェクトが範囲内に入ったら半透明化
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

        bool shouldBeTransparent = false;

        foreach (Collider col in players)
        {
            if (col == null || col == myCollider) continue;

            // 衝突無効化判定（タグベース）
            bool shouldIgnore = false;
            foreach (string tag in ignoreCollisionTags)
            {
                if (col.CompareTag(tag))
                {
                    shouldIgnore = true;
                    break;
                }
            }

            if (!ignoreState.ContainsKey(col) || ignoreState[col] != shouldIgnore)
            {
                Physics.IgnoreCollision(myCollider, col, shouldIgnore);
                ignoreState[col] = shouldIgnore;
            }

            // 半透明化判定（タグベース）
            if (enableTransparency)
            {
                foreach (string tag in transparentOnTags)
                {
                    if (col.CompareTag(tag))
                    {
                        shouldBeTransparent = true;
                        break;
                    }
                }
            }
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
