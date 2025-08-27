using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// �v���C���[�����͈͂ɓ������Ƃ���
/// �E�Փ˖����iPlayer���Ƃ�ON/OFF�\�j
/// �E���������i�@�\ON���̂݁A�������g + �w��I�u�W�F�N�g�Q�j
/// ���s���X�N���v�g
/// </summary>
public class ColorCheckerTrans : MonoBehaviour
{
    [Header("�� �v���C���[���m�{�b�N�X")]
    public Vector3 boxCenterOffset = Vector3.zero;
    public Vector3 boxSize = new Vector3(3f, 3f, 3f);
    public LayerMask playerLayer;

    [Header("�� �Փ˂̖�����")]
    public bool ignorePlayer1Red = true;
    public bool ignorePlayer2Blue = false;

    [Header("�� ���������@�\")]
    public bool enableTransparency = true;          // ���������@�\���̂�ON/OFF
    public bool transparentOnPlayer1Red = true;    // Player1�œ��������邩
    public bool transparentOnPlayer2Blue = true;   // Player2�œ��������邩
    [Range(0f, 1f)] public float transparentAlpha = 0.3f;

    [Header("�� �ǉ��Ŕ�����������I�u�W�F�N�g")]
    public GameObject[] extraTransparentObjects;

    private Collider myCollider;
    private Dictionary<Collider, bool> ignoreState = new Dictionary<Collider, bool>();

    // �����_���[�Ƃ��̃}�e���A���̌��F���Ǘ�
    private Renderer myRenderer;
    private Dictionary<Material, Color> myOriginalColors = new Dictionary<Material, Color>();
    private Dictionary<GameObject, Dictionary<Material, Color>> extraOriginalColors =
        new Dictionary<GameObject, Dictionary<Material, Color>>();

    void Start()
    {
        // �����̃R���C�_�[
        myCollider = GetComponent<Collider>();
        if (myCollider == null) Debug.LogError("Collider ������܂���I");

        // �����̃����_���[�ƃ}�e���A���o�^
        myRenderer = GetComponent<Renderer>();
        if (myRenderer != null)
        {
            foreach (Material mat in myRenderer.materials)
            {
                myOriginalColors[mat] = mat.color;
                SetMaterialToFade(mat); // ���炩���ߓ������Ή�
            }
        }

        // �ǉ��ΏۃI�u�W�F�N�g�̃����_���[�ƃ}�e���A���o�^
        foreach (var obj in extraTransparentObjects)
        {
            if (obj == null) continue;
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                var dict = new Dictionary<Material, Color>();
                foreach (Material mat in rend.materials)
                {
                    dict[mat] = mat.color;
                    SetMaterialToFade(mat);
                }
                extraOriginalColors[obj] = dict;
            }
        }
    }

    void Update()
    {
        // �͈͓��̃v���C���[���m
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

        // ������������
        bool shouldBeTransparent = false;
        if (enableTransparency)
        {
            if (player1InRange && transparentOnPlayer1Red)
                shouldBeTransparent = true;
            else if (player2InRange && transparentOnPlayer2Blue)
                shouldBeTransparent = true;
        }

        // �������g�̃}�e���A���X�V
        if (myRenderer != null)
        {
            foreach (Material mat in myRenderer.materials)
            {
                if (!myOriginalColors.ContainsKey(mat)) continue;
                Color c = myOriginalColors[mat];
                c.a = shouldBeTransparent ? transparentAlpha : 1f;
                mat.color = c;
            }
        }

        // �ǉ��I�u�W�F�N�g�̃}�e���A���X�V
        foreach (var kvp in extraOriginalColors)
        {
            GameObject obj = kvp.Key;
            if (obj == null) continue;
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend == null) continue;

            foreach (Material mat in rend.materials)
            {
                if (!kvp.Value.ContainsKey(mat)) continue;
                Color c = kvp.Value[mat];
                c.a = shouldBeTransparent ? transparentAlpha : 1f;
                mat.color = c;
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

    /// <summary>
    /// �}�e���A���𓧉߃��[�h�ɐ؂�ւ�
    /// </summary>
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
