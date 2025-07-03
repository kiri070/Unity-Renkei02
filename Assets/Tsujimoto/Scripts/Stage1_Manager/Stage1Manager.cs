using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Manager : MonoBehaviour
{
    SoundManager soundManager;
    SoundsList soundsList;
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();

        soundManager.OnPlayBGM(soundsList.stage1BGM); //BGM
    }
    
    void Update()
    {
        
    }
}
