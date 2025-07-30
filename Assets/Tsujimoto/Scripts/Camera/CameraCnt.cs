using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCnt : MonoBehaviour
{
    GameObject player1, player2; //プレイヤーのオブジェクト
    Vector3 baseOffset; // 中心からの初期オフセット
    Quaternion baseRotation; //初期回転値

    [Header("カメラの広がる値")]
    [Range(0.1f, 0.5f)]
    [SerializeField] float distanceFactor = 0.2f;

    float fixedCenterY; //カメラのy値

    PlayerMover mover1, mover2;

    // 現在のズーム倍率を管理する変数
    [Header("トランポリン時のカメラズームアウト")]
    [SerializeField][Range(3f, 5f)] float trampolineZoomout = 4f;
    float currentZoomFactor;
    float targetZoomFactor;
    float zoomSpeed = 5f; // ズームスピード（値を調整可）
    bool isShaking = false; //カメラをゆらしているかのフラウ

    void Start()
    {
        //プレイヤーのオブジェクトを格納
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");

        //プレイヤーのスクリプトを格納
        mover1 = player1.transform.GetComponent<PlayerMover>();
        mover2 = player2.transform.GetComponent<PlayerMover>();

        // プレイヤーの中心を基準にオフセットを計算
        Vector3 center = (player1.transform.position + player2.transform.position) / 2f;
        baseOffset = transform.position - center;

        //カメラの初期回転値
        baseRotation = transform.rotation;
        fixedCenterY = center.y;
    }

    //旧バージョン
    // void Update()
    // {
    //     //プレイヤー同志の中間地点を計算
    //     Vector3 center = (player1.transform.position + player2.transform.position) / 2f;
    //     center.y = fixedCenterY; //ジャンプで高さが変更されないように

    //     //プレイヤー同士の横距離の絶対値
    //     float horizontalDistance = Mathf.Abs(player1.transform.position.x - player2.transform.position.x);

    //     if (!mover1.useTrampoline && !mover2.useTrampoline)
    //     {
    //         //Vector3(0f, 横距離 * 倍率, -横距離 * 倍率)
    //         Vector3 zoomOffset = new Vector3(0f, horizontalDistance * distanceFactor, -horizontalDistance * distanceFactor);

    //         // 中心に追従 + オフセット + ズーム補正
    //         transform.position = center + baseOffset + zoomOffset;
    //         transform.rotation = baseRotation;
    //     }
    //     //誰かがトランポリンを使っていたら
    //     else if (mover1.useTrampoline || mover2.useTrampoline)
    //     {
    //         //Vector3(0f, 横距離 * 倍率, -横距離 * 倍率)
    //         Vector3 zoomOffset = new Vector3(0f, horizontalDistance * distanceFactor, -horizontalDistance * distanceFactor);

    //         // 中心に追従 + オフセット + ズーム補正
    //         transform.position = center + baseOffset + zoomOffset;
    //         transform.rotation = baseRotation;
    //     }

    // }
    void Update()
    {
        // プレイヤー中間地点
        Vector3 center = (player1.transform.position + player2.transform.position) / 2f;
        center.y = fixedCenterY;

        float horizontalDistance = Mathf.Abs(player1.transform.position.x - player2.transform.position.x);

        // トランポリン中ならズームアウト
        targetZoomFactor = (mover1.useTrampoline || mover2.useTrampoline) ? trampolineZoomout : 1f;
        currentZoomFactor = Mathf.Lerp(currentZoomFactor, targetZoomFactor, Time.deltaTime * zoomSpeed);

        Vector3 zoomOffset = new Vector3(0f, horizontalDistance * distanceFactor, -horizontalDistance * distanceFactor);
        zoomOffset *= currentZoomFactor;

        Vector3 targetPos = center + baseOffset + zoomOffset;

        if (!isShaking)
        {
            // 通常時は滑らかに追従
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, baseRotation, Time.deltaTime * 5f);
        }
        else
        {
            // 揺れ中は即座に位置を更新（Lerp無効）
            transform.position = targetPos;
            transform.rotation = baseRotation;
        }
    }


    //カメラを揺らす(揺れ時間, 揺れの大きさ)
    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        isShaking = true; //カメラを揺らすフラグを立てる
        float elapsed = 0f; //経過時間

        while (elapsed < duration)
        {
            //球体の中でランダムに点を発生させて移動
            transform.position = transform.position + Random.insideUnitSphere * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }

        isShaking = false; //カメラを揺らすフラグをオフ
    }
}
