using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 宝箱を持っているキャラクター色の地面を表示する
/// </summary>
public class ChangeGround : MonoBehaviour
{
    PlayerCnt pnt;
    private PlayerMover pm1, pm2;
    List<GameObject> ground_Red = new List<GameObject>();
    List<GameObject> ground_Blue = new List<GameObject>();
    void Start()
    {
        pnt = FindObjectOfType<PlayerCnt>();
        pm1 = GameObject.Find("Player1").GetComponent<PlayerMover>();
        pm2 = GameObject.Find("Player2").GetComponent<PlayerMover>();


        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        //表示を切り替える赤と青の地面を取得
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Ground_Red"))
                ground_Red.Add(obj);

            if (obj.name.Contains("Ground_Blue"))
                ground_Blue.Add(obj);
        }

        //初期は非表示
        foreach (GameObject ground in ground_Red)
        {
            ground.GetComponent<Collider>().enabled = false;
            ground.GetComponent<Renderer>().enabled = false;
        }
        foreach (GameObject ground in ground_Blue)
        {
            ground.GetComponent<Collider>().enabled = false;
            ground.GetComponent<Renderer>().enabled = false;
        }
    }

    void Update()
    {
        //赤のキャラが宝箱を持っていたら
        if (pm1.heldObject != null)
        {
            if (pnt.isPlayer1BringObj)
            {
                //赤を表示
                foreach (GameObject ground in ground_Red)
                {
                    ground.GetComponent<Collider>().enabled = true;
                    ground.GetComponent<Renderer>().enabled = true;
                }
                //青を非表示
                foreach (GameObject ground in ground_Blue)
                {
                    ground.GetComponent<Collider>().enabled = false;
                    ground.GetComponent<Renderer>().enabled = false;
                }
            }
            else if (!pnt.isPlayer1BringObj)
            {
                //赤を表示
                foreach (GameObject ground in ground_Red)
                {
                    ground.GetComponent<Collider>().enabled = false;
                    ground.GetComponent<Renderer>().enabled = false;
                }
                //青を非表示
                foreach (GameObject ground in ground_Blue)
                {
                    ground.GetComponent<Collider>().enabled = false;
                    ground.GetComponent<Renderer>().enabled = false;
                }
            }
        }
        //青のキャラが宝箱を持っていたら
        else if (pm2.heldObject != null)
        {

            if (pnt.isPlayer2BringObj)
            {
                //青を表示
                foreach (GameObject ground in ground_Blue)
                {
                    ground.GetComponent<Collider>().enabled = true;
                    ground.GetComponent<Renderer>().enabled = true;
                }
                //赤を非表示
                foreach (GameObject ground in ground_Red)
                {
                    ground.GetComponent<Collider>().enabled = false;
                    ground.GetComponent<Renderer>().enabled = false;
                }
            }
            else if (!pnt.isPlayer2BringObj)
            {
                //青を表示
                foreach (GameObject ground in ground_Blue)
                {
                    ground.GetComponent<Collider>().enabled = false;
                    ground.GetComponent<Renderer>().enabled = false;
                }
                //赤を非表示
                foreach (GameObject ground in ground_Red)
                {
                    ground.GetComponent<Collider>().enabled = false;
                    ground.GetComponent<Renderer>().enabled = false;
                }
            }
        }
    }
}
