using UnityEngine;
using System.Collections;

/// <summary>
/// 砲台本体のスクリプト。弾の発射と首振りを管理する。
/// 首振りは外部スイッチからON/OFFされるが、発射後はクールタイム中に自動で首振りを停止する。
/// </summary>
public class CannonShooter : MonoBehaviour
{
    [Header("発射位置（砲口の先端）")]
    [SerializeField] private Transform firePoint;

    [Header("弾のPrefab")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("発射速度")]
    [SerializeField] private float bulletSpeed = 30f;

    [Header("クールタイム（秒）")]
    [SerializeField] private float reloadTime = 2f;

    [Header("検知ボックスの中心オフセット")]
    [SerializeField] private Vector3 boxCenterOffset = Vector3.zero;

    [Header("検知ボックスのサイズ")]
    [SerializeField] private Vector3 boxSize = new Vector3(3f, 3f, 3f);

    [Header("検知対象のレイヤー")]
    [SerializeField] private LayerMask playerLayer;

    [Header("左右に振る範囲（度）")]
    [SerializeField] private float swingAngle = 30f;

    [Header("首振り速度（度/秒）")]
    [SerializeField] private float swingSpeed = 30f;

    [Header("首振りさせる対象（通常は自分自身）")]
    [SerializeField] private Transform cannonBase;

    [Tooltip("外部スイッチが制御する首振りON/OFFフラグ")]
    [HideInInspector] public bool isSwinging = false;

    private bool isReloading = false;       // クールタイム中かどうか
    private bool wasSwingingBeforeFire = false; // 発射前の首振り状態を保存
    private float currentAngle = 0f;        // 現在の首振り角度
    private int swingDirection = 1;         // 首振り方向（1か-1）

    void Update()
    {
        // プレイヤーを検知して発射する
        if (!isReloading)
        {
            Vector3 boxCenter = transform.position + boxCenterOffset;

            Collider[] hits = Physics.OverlapBox(
                boxCenter,
                boxSize * 0.5f,
                Quaternion.identity,
                playerLayer
            );

            foreach (var col in hits)
            {
                if (col.CompareTag("Player1") || col.CompareTag("Player2"))
                {
                    Fire();
                    StartCoroutine(Reload());
                    break;
                }
            }
        }

        // 首振り処理（クールタイム中は止める）
        if (isSwinging && !isReloading)
        {
            Swing();
        }
    }

    /// <summary>
    /// 弾を生成して発射する
    /// </summary>
    private void Fire()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogWarning("FirePoint または BulletPrefab が設定されていません");
            return;
        }

        // 弾を生成
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Rigidbody に速度を設定
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // 必要に応じてON
            rb.velocity = firePoint.forward.normalized * bulletSpeed;
        }

        // 1秒後に弾を破壊
        Destroy(bullet, 1f);
    }

    /// <summary>
    /// クールタイム処理（首振りも一時停止）
    /// </summary>
    private IEnumerator Reload()
    {
        isReloading = true;

        // 現在の首振り状態を保存して首振り停止
        wasSwingingBeforeFire = isSwinging;
        isSwinging = false;

        yield return new WaitForSeconds(reloadTime);

        isReloading = false;

        // 元の状態がONなら再開
        if (wasSwingingBeforeFire)
        {
            isSwinging = true;
        }
    }

    /// <summary>
    /// 左右に首を振る
    /// </summary>
    private void Swing()
    {
        // 現在の角度を更新
        currentAngle += swingDirection * swingSpeed * Time.deltaTime;

        // 振り幅を超えたら方向を反転
        if (Mathf.Abs(currentAngle) >= swingAngle)
        {
            swingDirection *= -1;
            currentAngle = Mathf.Clamp(currentAngle, -swingAngle, swingAngle);
        }

        // 対象を回転
        if (cannonBase != null)
        {
            cannonBase.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
        }
    }

    /// <summary>
    /// ギズモで検知ボックスを可視化する
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Vector3 boxCenter = transform.position + boxCenterOffset;
        Gizmos.DrawCube(boxCenter, boxSize);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
