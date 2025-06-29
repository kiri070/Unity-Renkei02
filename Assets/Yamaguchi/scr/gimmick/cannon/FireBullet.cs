using UnityEngine;
public class FireBullet : MonoBehaviour
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
    void Update()
    {
        // スペースキーが押されたかを判定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 弾を発射する場所を取得
            Vector3 bulletPosition = firingPoint.transform.position;
            // 上で取得した場所に、"bullet"のPrefabを出現させる。Bulletの向きはMuzzleのローカル値と同じにする（3つ目の引数）
            GameObject newBullet = Instantiate(bullet, bulletPosition, this.gameObject.transform.rotation);
            // 出現させた弾のup(Y軸方向)を取得（MuzzleのローカルY軸方向のこと）
            Vector3 direction = newBullet.transform.up;
            // 弾の発射方向にnewBallのY方向(ローカル座標)を入れ、弾オブジェクトのrigidbodyに衝撃力を加える
            newBullet.GetComponent<Rigidbody>().AddForce(direction * speed, ForceMode.Impulse);
            // 出現させた弾の名前を"bullet"に変更
            newBullet.name = bullet.name;
            // 出現させた弾を0.8秒後に消す
            Destroy(newBullet, 0.8f);
        }
    }
}