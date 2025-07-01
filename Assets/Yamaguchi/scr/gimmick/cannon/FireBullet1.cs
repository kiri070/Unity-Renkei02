using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
public class FireBullet1 : MonoBehaviour
{
    [SerializeField]
    [Tooltip("弾の発射場所")]
    private GameObject firingPoint;
    [SerializeField]
    [Tooltip("弾")]
    private GameObject bullet;
    [SerializeField]
    [Tooltip("弾の速さ")]
    private float speed = 30f;
    // Update is called once per frame

    //public float canonx = 200;
    //public float canony = 4;

    //public float canonz = 0;

    // ▼ ボックスの中心位置を、オブジェクトの位置からどれだけずらすか（例：頭の上とか）
    public Vector3 boxCenterOffset = Vector3.zero;

    // ▼ ボックスのサイズ（幅, 高さ, 奥行き）を設定。OverlapBoxではこれを半分にして使う。
    public Vector3 boxSize = new Vector3(3f, 3f, 3f);

    // ▼ プレイヤーが所属するレイヤー。無関係なもの（地面など）を除外できる
    public LayerMask playerLayer;
    private Collider myCollider;
    bool reloading; // リロード中であるか否かを表すフラグ

    public float swingAngle = 100f;    // 振れ幅（例：±30度）
    public float swingSpeed = 1f;     // 往復の速さ（1秒で往復するなら1）

    private float baseY;              // 元のY角度
    public bool isSwinging = false;  // スイング中フラグ
    private float swingTimer = 0f;    // 経過時間

    void Start()
    {
        reloading = false; //クールタイム

        baseY = transform.eulerAngles.y;
    }


    void Update()
    {
        if (reloading) return; // 🔑 リロード中なら何もしない

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
                Fire();
                // リロード開始
                StartCoroutine(Reload());
                break;

            }
        }

        if (isSwinging)
        {
            swingTimer += Time.deltaTime * swingSpeed;

            // sin波を使って往復
            float offset = Mathf.Sin(swingTimer * Mathf.PI * 2) * (swingAngle * 0.5f);
            transform.rotation = Quaternion.Euler(0, baseY + offset, 0);
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

    void Fire()
    {
        if (!reloading)
        {
            // 弾を発射する場所を取得
            Vector3 bulletPosition = firingPoint.transform.position;
            // 上で取得した場所に、"bullet"のPrefabを出現させる。Bulletの向きはMuzzleのローカル値と同じにする（3つ目の引数）
            GameObject newBullet = Instantiate(bullet, bulletPosition, this.gameObject.transform.rotation);
            // 出現させた弾のup(Y軸方向)を取得（MuzzleのローカルY軸方向のこと）
            Vector3 direction = newBullet.transform.up;
            // x軸にちょっと足す（例：0.2 上向き）
            direction += Vector3.forward * 1.5f;
            // 弾の発射方向にnewBallのY方向(ローカル座標)を入れ、弾オブジェクトのrigidbodyに衝撃力を加える
            newBullet.GetComponent<Rigidbody>().AddForce(direction * speed, ForceMode.Impulse);
            //newBullet.GetComponent<Rigidbody>().AddForce(canonx, canony, canonz, ForceMode.Impulse);
            // 出現させた弾の名前を"bullet"に変更
            newBullet.name = bullet.name;
            // 出現させた弾を0.8秒後に消す
            Destroy(newBullet, 1f);
        }
    }

    private IEnumerator Reload()
    {
        // リロードのアニメーション開始 etc...
        Debug.Log("リロード開始");
        reloading = true;

        // ２秒待機
        yield return new WaitForSeconds(2);
        Debug.Log("リロード完了");
        reloading = false;
    }

    public void Rotate()
    {
        if (!isSwinging)
        {
            swingTimer = 0f;
            baseY = transform.eulerAngles.y; // 開始時の角度を基準にする
        }
    }
}