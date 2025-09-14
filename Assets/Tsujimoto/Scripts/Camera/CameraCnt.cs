using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraCnt : MonoBehaviour
{
    GameObject player1, player2;
    Vector3 baseOffset;
    Quaternion baseRotation;

    [Header("カメラの広がる値")]
    [Range(0.1f, 0.5f)]
    public float distanceFactor = 0.2f;

    float fixedCenterY;

    PlayerMover mover1, mover2;

    [Header("トランポリン時のカメラズームアウト")]
    [SerializeField][Range(3f, 5f)] float trampolineZoomout = 4f;
    float currentZoomFactor;
    float targetZoomFactor;
    float zoomSpeed = 5f;
    bool isShaking = false;

    [Header("ズームアウトエリアのカメラズームアウト")]
    [SerializeField] [Range(3f, 30f)] float zoomOutArea_Zoomout = 3f;

    [Header("ベルトコンベアエリアのカメラズームアウト")]
    [SerializeField] [Range(3f, 30f)] float zoomOutArea_Beltconveyor = 3f;

    // レイキャスト用
    [Header("壁透過設定")]
    [SerializeField] Material transparentMat;   // 半透明マテリアル
    Dictionary<GameObject, Material> originalMats = new Dictionary<GameObject, Material>(); // 元のマテリアルを保存

    // プレイヤーごとに「前フレームで透過してた壁」を記録
    List<GameObject> prevHitObjectsP1 = new List<GameObject>();
    List<GameObject> prevHitObjectsP2 = new List<GameObject>();

    void Start()
    {
        // プレイヤー取得
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");

        mover1 = player1.GetComponent<PlayerMover>();
        mover2 = player2.GetComponent<PlayerMover>();

        // 初期オフセット計算
        Vector3 center = (player1.transform.position + player2.transform.position) / 2f;
        baseOffset = transform.position - center;

        baseRotation = transform.rotation;
        fixedCenterY = center.y;
    }

    //void Update()
    //{
    //    // カメラ追従
    //    // プレイヤー中間地点
    //    Vector3 center = (player1.transform.position + player2.transform.position) / 2f;
    //    center.y = fixedCenterY;

    //    // 特定エリアならカメラのYを持ち上げる
    //    if (mover1.isHighCameraPos && mover2.isHighCameraPos)
    //    {
    //        center.y += 30f; // 上げたい分だけ調整
    //    }

    //    float horizontalDistance = Mathf.Abs(player1.transform.position.x - player2.transform.position.x);

    //    // ズームアウトエリアなら、最小距離を保証してズームインを抑える
    //    if (mover1.isZoomOutPos || mover2.isZoomOutPos)
    //    {
    //        horizontalDistance = Mathf.Max(horizontalDistance, 15f);
    //    }

    //    // トランポリン中ならズームアウト
    //    targetZoomFactor = (mover1.useTrampoline || mover2.useTrampoline) ? trampolineZoomout : 1f;
    //    currentZoomFactor = Mathf.Lerp(currentZoomFactor, targetZoomFactor, Time.deltaTime * zoomSpeed);

    //    //ズームアウトエリアなら
    //    targetZoomFactor = (mover1.isZoomOutPos && !mover1.useTrampoline || mover2.isZoomOutPos && !mover2.useTrampoline) ? zoomOutArea_Zoomout : 1f;
    //    currentZoomFactor = Mathf.Lerp(currentZoomFactor, targetZoomFactor, Time.deltaTime * zoomSpeed);

    //    //ベルトコンベアのズームアウトエリアなら
    //    targetZoomFactor = (mover1.isBeltconveyorPos || mover2.isBeltconveyorPos) ? zoomOutArea_Beltconveyor : 1f;
    //    currentZoomFactor = Mathf.Lerp(currentZoomFactor, targetZoomFactor, Time.deltaTime * zoomSpeed);

    //    Vector3 zoomOffset = new Vector3(0f, horizontalDistance * distanceFactor, -horizontalDistance * distanceFactor);
    //    zoomOffset *= currentZoomFactor;

    //    Vector3 targetPos = center + baseOffset + zoomOffset;



    //    if (!isShaking)
    //    {
    //        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
    //        transform.rotation = Quaternion.Lerp(transform.rotation, baseRotation, Time.deltaTime * 5f);
    //    }
    //    else
    //    {
    //        transform.position = targetPos;
    //        transform.rotation = baseRotation;
    //    }

    //    // 壁透過処理
    //    HandleWallTransparency(player1, prevHitObjectsP1);
    //    HandleWallTransparency(player2, prevHitObjectsP2);
    //}

    void Update()
    {
        // ===== プレイヤー中間地点 =====
        Vector3 center = (player1.transform.position + player2.transform.position) / 2f;
        center.y = fixedCenterY;

        // 特定エリアならカメラのYを持ち上げる
        if (mover1.isHighCameraPos && mover2.isHighCameraPos)
        {
            center.y += 30f; // 上げたい分だけ調整
        }

        //プレイヤー間の横の距離
        float horizontalDistance = Mathf.Abs(player1.transform.position.x - player2.transform.position.x);

        // ===== ズーム倍率の決定 =====
        if (mover1.isZoomoutPos && mover1.zoomOutSetting != null)
        {
            targetZoomFactor = mover1.zoomOutSetting.Zoomout;
        }
        else if (mover2.isZoomoutPos && mover2.zoomOutSetting != null)
        {
            targetZoomFactor = mover2.zoomOutSetting.Zoomout;
        }
        else if (mover1.useTrampoline || mover2.useTrampoline)
        {
            targetZoomFactor = trampolineZoomout;
        }
        else
        {
            targetZoomFactor = 1f; // 通常倍率
        }

        currentZoomFactor = Mathf.Lerp(currentZoomFactor, targetZoomFactor, Time.deltaTime * zoomSpeed);

        // ===== カメラのズームオフセット計算 =====
        Vector3 zoomOffset = new Vector3(
            0f,
            horizontalDistance * distanceFactor,
            -horizontalDistance * distanceFactor
        );

        zoomOffset *= currentZoomFactor;

        // ZOffset（ZoomOutSetting 側で設定されている補正値）を適用
        if (mover1.isZoomoutPos && mover1.zoomOutSetting != null)
        {
            zoomOffset.z += mover1.zoomOutSetting.ZOffset;
        }
        else if (mover2.isZoomoutPos && mover2.zoomOutSetting != null)
        {
            zoomOffset.z += mover2.zoomOutSetting.ZOffset;
        }

        Vector3 targetPos = center + baseOffset + zoomOffset;

        // ===== カメラの移動・回転 =====
        if (!isShaking)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, baseRotation, Time.deltaTime * 5f);
        }
        else
        {
            transform.position = targetPos;
            transform.rotation = baseRotation;
        }

        // ===== 壁透過処理 =====
        HandleWallTransparency(player1, prevHitObjectsP1);
        HandleWallTransparency(player2, prevHitObjectsP2);
    }

    /// <summary>
    /// カメラからプレイヤーへのラインにある壁を透過
    /// </summary>
    void HandleWallTransparency(GameObject player, List<GameObject> prevHits)
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.transform.position);

        int mask = LayerMask.GetMask("Wall", "Player");
        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, dist, mask);

        // 距離順に並べる
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        List<GameObject> currentHits = new List<GameObject>();

        foreach (RaycastHit hit in hits)
        {
            // プレイヤーに当たったら、それ以降の壁は無視
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                break;

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (!originalMats.ContainsKey(hit.collider.gameObject))
                    {
                        originalMats[hit.collider.gameObject] = rend.material;
                    }
                    rend.material = transparentMat; // 半透明に切り替え
                    currentHits.Add(hit.collider.gameObject);
                }
            }
        }

        // 隠れていない壁を戻す
        foreach (GameObject prev in prevHits.Except(currentHits).ToList())
        {
            if (prev != null && originalMats.ContainsKey(prev))
            {
                Renderer rend = prev.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = originalMats[prev];
                }
            }
        }

        prevHits.Clear();
        prevHits.AddRange(currentHits);
    }


    /// <summary>
    /// カメラを揺らす処理
    /// </summary>
    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = transform.position + Random.insideUnitSphere * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }

        isShaking = false;
    }
}
