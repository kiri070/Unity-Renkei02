using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCntTest : MonoBehaviour
{
    [Header("追従範囲の半径")][SerializeField] private float detectionRadius = 5f; // 追従開始の半径
    [Header("プレイヤーのレイヤー")][SerializeField] private LayerMask playerLayer; // Playerレイヤーを指定（推奨）

    public Collider[] hits; //hitしたコライダーを格納する配列
    GameObject playerObj; //範囲内のプレイヤーを格納する
    [Header("球を発射する位置")][SerializeField] Transform shotPos;
    [Header("発射する球")][SerializeField] GameObject cannonBall;
    [Header("エイム時,球発射クールタイム")][SerializeField] float AimCoolTime = 5f;
    [Header("一定間隔時,球発射クールタイム")][SerializeField] float coolTime = 3f;
    [Header("球の速度")][SerializeField] float shotForce = 1000f;
    [Header("プレイヤーの方向に打つかどうか")]
    [Tooltip("チェックを入れると狙い撃ち")][SerializeField] bool isAimPlayer = false;

    [Header("エフェクト")]
    [Tooltip("発射時")][SerializeField] GameObject shotEffect;

    bool canAimShot = true; //球をエイム発射できるかどうか
    bool canShot = true; //球を発射できるかどうか

    SoundManager soundManager;
    SoundsList soundsList;
    AudioSource se3DAudioSource; //3d音声のオーディオソース

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
        // 自分のオブジェクトの AudioSource（3D音対応）を使う
        se3DAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //プレイヤーを狙って打つ場合
        if (isAimPlayer)
        {
            // 指定した範囲内にプレイヤーがいるか調べる
            hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

            //範囲内なら
            if (hits.Length > 0)
            {
                //壁に隠れていない時に撃つ
                HidePlayerCheck();
            }

        }
        //一定間隔で撃つ場合
        else if (!isAimPlayer)
        {
            if (canShot)
            {
                Shot();
            }
        }
    }

    //壁に隠れているか判定して行動する処理
    void HidePlayerCheck()
    {
        RaycastHit hit;
        Vector3 origin = shotPos.transform.position; //Rayを飛ばす位置
        Vector3 direction = (hits[0].transform.position - origin).normalized; //プレイヤーの方向
        int mask = LayerMask.GetMask("Player", "Wall"); //判定するレイヤー
        if (Physics.Raycast(shotPos.transform.position, direction, out hit, mask))
        {
            //隠れていなければ
            if (hit.collider.CompareTag("Player1") || hit.collider.CompareTag("Player2"))
            {
                Vector3 direction1 = (hits[0].transform.position - transform.position).normalized;// 向くべき方向を計算（y軸だけ回転）
                direction1.y = 0f; // 上下回転しないように

                //回転処理
                if (direction1 != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction1);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
                }
                //球を発射
                if (canAimShot)
                {
                    AimShot();
                }
            }
            //壁に隠れていたら
            else if (hit.collider.CompareTag("Wall"))
            {
                return;
            }
        }
    }

    //球をエイム発射する処理
    void AimShot()
    {
        GameObject ball = Instantiate(cannonBall, shotPos.transform.position, cannonBall.transform.rotation);

        //プレイヤーの方向を計算
        Vector3 direction = (hits[0].transform.position - shotPos.position).normalized;
        Vector3 force = direction * shotForce + Vector3.up;
        //球に力を加える
        ball.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        //エフェクト
        Instantiate(shotEffect, shotPos.transform.position, shotEffect.transform.rotation);

        //効果音
        OnPlaySE(soundsList.shotCannonSE);

        canAimShot = false;
        StartCoroutine(AimShotCoolTime()); //クールタイム処理
    }

    //球を一定間隔で発射する処理
    void Shot()
    {
        GameObject ball = Instantiate(cannonBall, shotPos.transform.position, cannonBall.transform.rotation);

        //前方向を取得
        Vector3 direction = transform.forward;
        //球に力を加える
        ball.GetComponent<Rigidbody>().AddForce(direction * shotForce);

        //エフェクト
        Instantiate(shotEffect, shotPos.transform.position, shotEffect.transform.rotation);

        //効果音
        OnPlaySE(soundsList.shotCannonSE);

        canShot = false;
        StartCoroutine(ShotCoolTime());
    }

    //エイム発射のクールタイム
    IEnumerator AimShotCoolTime()
    {
        yield return new WaitForSeconds(AimCoolTime);
        canAimShot = true;
    }
    //一定間隔のクールタイム
    IEnumerator ShotCoolTime()
    {
        yield return new WaitForSeconds(coolTime);
        canShot = true;
    }
    //範囲を描画(開発中のみ)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    //効果音を鳴らす関数(呼び出し時volumeは省略可)
    void OnPlaySE(AudioClip audioClip, float volume = 1f)
    {
        if (audioClip == null)
            return;
        // 効果音（距離減衰 & UI音量に合わせる）
        float seVolume = soundManager.seVolumeSlider.value;
        se3DAudioSource.PlayOneShot(soundsList.shotCannonSE, seVolume);
    }
}
