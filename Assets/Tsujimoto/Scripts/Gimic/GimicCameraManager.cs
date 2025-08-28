using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicCameraManager : MonoBehaviour
{
    GameObject player1, player2; //プレイヤーのオブジェクト
    Vector3 baseOffset; // 中心からの初期オフセット
    Quaternion baseRotation; //初期回転値

    [Header("カメラの広がる値")]
    [Range(0.1f, 0.5f)]
    [SerializeField] float distanceFactor = 0.2f;

    float fixedCenterY; //カメラのy値

    void Start()
    {
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");

        // プレイヤーの中心を基準にオフセットを計算
        Vector3 center = (player1.transform.position + player2.transform.position) / 2f;
        baseOffset = transform.position - center;

        //カメラの初期回転値
        baseRotation = transform.rotation;
        fixedCenterY = center.y;
    }

    void Update()
    {
        // 2人のプレイヤーのX座標の中間を計算
        float centerZ = (player1.transform.position.z + player2.transform.position.z) / 2f;

        // 現在のX/Yを維持しつつ、Zだけ変更
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, centerZ - 30f);

        // カメラを移動
        transform.position = newPosition;

        // 回転は固定
        transform.rotation = baseRotation;
    }

    //カメラを揺らす(揺れ時間, 揺れの大きさ)
    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        float elapsed = 0f; //経過時間

        while (elapsed < duration)
        {
            //球体の中でランダムに点を発生させて移動
            transform.position = transform.position + Random.insideUnitSphere * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
