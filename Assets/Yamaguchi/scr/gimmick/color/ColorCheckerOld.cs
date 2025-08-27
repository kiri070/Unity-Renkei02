using UnityEngine;
using System.Collections.Generic;

public class ColorCheckerOld : MonoBehaviour
{
    // ▼ ボックスの中心位置を、オブジェクトの位置からどれだけずらすか（例：頭の上とか）
    public Vector3 boxCenterOffset = Vector3.zero;

    // ▼ ボックスのサイズ（幅, 高さ, 奥行き）を設定。OverlapBoxではこれを半分にして使う。
    public Vector3 boxSize = new Vector3(3f, 3f, 3f);

    // ▼ プレイヤーが所属するレイヤー。無関係なもの（地面など）を除外できる
    public LayerMask playerLayer;

    // ▼ タグ「Player1」との衝突を無視する（true = すり抜ける）
    public bool ignorePlayer1 = true;

    // ▼ タグ「Player2」との衝突を無視する（false = ぶつかる）
    public bool ignorePlayer2 = false;

    // 自分のコライダーを格納する
    private Collider myCollider;

    // どの相手と衝突を無視しているかを記録する辞書（無駄な再設定を防ぐため）
    private Dictionary<Collider, bool> ignoreState = new Dictionary<Collider, bool>();

    void Start()
    {
        // 自分にColliderが付いているか確認し、なければエラーを出す
        myCollider = GetComponent<Collider>();
        if (myCollider == null)
        {
            Debug.LogError("このオブジェクトにColliderがありません！");
        }
    }

    void Update()
    {
        // ボックスの中心位置を計算（自分の位置＋オフセット）
        Vector3 boxCenter = transform.position + boxCenterOffset;

        // 指定した範囲（ボックス）内にあるプレイヤーのColliderをすべて取得
        Collider[] players = Physics.OverlapBox(
            boxCenter,
            boxSize * 0.5f,       // 半サイズで指定する必要あり！
            Quaternion.identity,  // ボックスの回転（ここでは無回転）
            playerLayer           // 検出対象のレイヤー
        );

        // 検出されたプレイヤーたちを1つずつチェック
        foreach (Collider col in players)
        {
            if (col == null || col == myCollider)
                continue; // 自分自身はスキップ

            // そのプレイヤーとの衝突を無視するべきかどうかをタグで判断
            bool shouldIgnore = false;

            if (col.CompareTag("Player1"))
                shouldIgnore = ignorePlayer1;
            else if (col.CompareTag("Player2"))
                shouldIgnore = ignorePlayer2;

            // 状態が変わっている場合だけIgnoreCollisionを呼ぶ（パフォーマンス最適化）
            if (!ignoreState.ContainsKey(col) || ignoreState[col] != shouldIgnore)
            {
                // 実際に衝突を無視する/しないを切り替える
                Physics.IgnoreCollision(myCollider, col, shouldIgnore);

                // 状態を記録しておく
                ignoreState[col] = shouldIgnore;
            }
        }
    }

    // ▼ Unityエディタ上で、検出範囲のボックスを見えるように描く関数
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.25f); // 薄いシアン（透明）
        Vector3 boxCenter = transform.position + boxCenterOffset;
        Gizmos.DrawCube(boxCenter, boxSize);      // 塗りつぶしのキューブ
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boxCenter, boxSize);  // 枠線だけのキューブ
    }
}