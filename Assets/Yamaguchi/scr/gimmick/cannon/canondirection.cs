using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canondirection : MonoBehaviour
{
    FireBullet1 firebullet1;
    public GameObject _parent;
    // Start is called before the first frame update
    void Start()
    {
        firebullet1 = _parent.GetComponent<FireBullet1>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player1") || other.gameObject.CompareTag("Player2"))
        {
            firebullet1.isSwinging = true;
            firebullet1.Rotate();
            Debug.Log("回転します");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player1") || other.gameObject.CompareTag("Player2"))
        {
            firebullet1.isSwinging = false;
            Debug.Log("回転を止めます");
        }
    }
}
