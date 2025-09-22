using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRock : MonoBehaviour
{
    [Header("岩のスポーンポイント(空のオブジェクト)")]
    [SerializeField] List<GameObject> spawnPoints;
    [Header("岩のオブジェクト")]
    [SerializeField] GameObject rockObj;
    [Header("スポーン間隔")]
    [SerializeField] int spawnTime = 3;
    [Header("岩が消えるまでの時間")]
    public int deleteTime = 3;

    bool canSpawn = true; //スポーンできるかどうか

    void Update()
    {
        //スポーンさせる
        if(canSpawn)
        {
            int rnd = Random.Range(0, spawnPoints.Count);
            GameObject summonRock = Instantiate(rockObj, spawnPoints[rnd].transform.position, rockObj.transform.rotation);
            StartCoroutine(SpawnFlag());
            canSpawn = false;
        }
    }

    //スポーンフラグをリセットするコルーチン
    IEnumerator SpawnFlag()
    {
        yield return new WaitForSeconds(spawnTime);
        canSpawn = true;
    }
}
