using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canon : MonoBehaviour
{
    // ▼ ボックスの中心位置を、オブジェクトの位置からどれだけずらすか（例：頭の上とか）
    public Vector3 boxCenterOffset = Vector3.zero;

    // ▼ ボックスのサイズ（幅, 高さ, 奥行き）を設定。OverlapBoxではこれを半分にして使う。
    public Vector3 boxSize = new Vector3(3f, 3f, 3f);

    // ▼ プレイヤーが所属するレイヤー。無関係なもの（地面など）を除外できる
    public LayerMask playerLayer;
    private Collider myCollider;

    canonBall canonball;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
            if (col.CompareTag("Player1") || col.CompareTag("Player2"))
            {
                Debug.Log("Fire!!");
                // インスペクター上で設定したBulletのプレハブを生成する→その後にFire()実行
                //canonball.Fire();
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
