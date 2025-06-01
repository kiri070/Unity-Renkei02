using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class StageSelectManager : MonoBehaviour
{

    //ステージ1に変遷するボタン
    public void OnStartStage1Button()
    {
        SceneManager.LoadScene("GameScene01");
    }
}
