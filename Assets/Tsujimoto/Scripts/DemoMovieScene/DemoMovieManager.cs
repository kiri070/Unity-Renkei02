using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoMovieManager : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        //任意のキーを押すとシーン変遷
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
        {
            SceneManager.LoadScene("Title");
        }
    }
}
