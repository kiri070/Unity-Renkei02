using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltScroll : MonoBehaviour
{
    [Header("対象Renderer（未指定なら自分）")]
    public Renderer targetRenderer;

    [Header("動かすマテリアルのインデックス (0開始)")]
    public int materialIndex = 0;

    [Header("流れる速さ (UV/s)")]
    public Vector2 speed = new Vector2(1f, 0f);

    [Header("スロット自動検出 (URP/HDRP:_BaseMap / Built-in:_MainTex)")]
    public bool autoDetectSlot = true;

    [Tooltip("自動検出を切る場合に使う。URP/HDRPは _BaseMap、Built-inは _MainTex")]
    public string textureSlot = "_BaseMap";

    Vector2 offset;
    Vector2 tiling = Vector2.one;
    MaterialPropertyBlock mpb;

    int stId;
    string resolvedSlot;

    static readonly string[] Candidates = { "_BaseMap", "_MainTex" };

    void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();

        // スロット決定
        if (autoDetectSlot)
        {
            var mat = targetRenderer.sharedMaterials.Length > materialIndex
                      ? targetRenderer.sharedMaterials[materialIndex]
                      : null;

            foreach (var c in Candidates)
            {
                if (mat != null && mat.HasProperty(c))
                {
                    resolvedSlot = c;
                    break;
                }
            }
            if (string.IsNullOrEmpty(resolvedSlot))
                resolvedSlot = textureSlot; // 最後の手段
        }
        else
        {
            resolvedSlot = textureSlot;
        }
        stId = Shader.PropertyToID(resolvedSlot + "_ST");

        // インデックス範囲ガード
        if (materialIndex < 0 || materialIndex >= targetRenderer.sharedMaterials.Length)
        {
            Debug.LogError($"{name}: materialIndex が範囲外です。Rendererのマテリアル数={targetRenderer.sharedMaterials.Length}");
            enabled = false;
        }
    }

    void Update()
    {
        offset += speed * Time.deltaTime;

        // _ST = (tilingX, tilingY, offsetX, offsetY)
        mpb.Clear();
        mpb.SetVector(stId, new Vector4(tiling.x, tiling.y, offset.x, offset.y));

        // ★ここがポイント：第2引数に materialIndex を渡す
        targetRenderer.SetPropertyBlock(mpb, materialIndex);
    }
}
