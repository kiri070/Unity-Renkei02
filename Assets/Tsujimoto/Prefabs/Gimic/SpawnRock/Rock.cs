using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    SpawnRock spawnRock;
    private void Start()
    {
        spawnRock = FindObjectOfType<SpawnRock>();
        StartCoroutine(DeleteRock());
    }
    private void OnCollisionEnter(Collision other)
    {
        //プレイヤー
        if (other.gameObject.CompareTag("Player1") || other.gameObject.CompareTag("Player2"))
        {
            PlayerMover mover = other.gameObject.GetComponent<PlayerMover>(); // まとめて取得
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();

            mover.canMove = false;
            Vector3 dir = (other.transform.position - transform.position).normalized; //方向を算出
            rb.AddForce(dir * 5f, ForceMode.Impulse);

            //プレイヤー側で実行する
            mover.StartCoroutine(mover.RecoveryKnockback(mover.recoveryKnockbackTime));
        }
        //宝箱に衝突したら
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj"))
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();

            Vector3 dir = (other.gameObject.transform.position - transform.position).normalized; //方向を算出
            rb.AddForce(dir * 50f, ForceMode.Impulse);
        }
    }

    //時間経過で岩を削除
    IEnumerator DeleteRock()
    {
        yield return new WaitForSeconds(spawnRock.deleteTime);
        Destroy(this.gameObject);
    }
}
