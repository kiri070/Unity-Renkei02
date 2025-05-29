using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("BGM,SEを鳴らすAudioSource")]
    public AudioSource bgmAudioSource;
    public AudioSource seAudioSource;

    [Header("BGM,SEのSlider")]
    public Slider bgmVolumeSlider;
    public Slider seVolumeSlider;

    SoundsList soundsList; //SoundsListのインスタンス
    void Start()
    {
        //コンポーネント取得
        soundsList = GetComponent<SoundsList>();

        OnPlayBGM(soundsList.gameBGM); //初期のBGM

        //BGM,SEの音量を取り出し
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        seVolumeSlider.value = PlayerPrefs.GetFloat("SEVolume");
    }

    //効果音を鳴らす関数
    public void OnPlaySE(AudioClip audioClip)
    {
        //音がなっていた場合
        if (seAudioSource.isPlaying)
            seAudioSource.Stop();

        seAudioSource.clip = audioClip;
        seAudioSource.Play();
    }

    public void OnPlayBGM(AudioClip audioClip)
    {
        if (bgmAudioSource.isPlaying)
            bgmAudioSource.Stop();

        bgmAudioSource.clip = audioClip;
        bgmAudioSource.Play();
    }

    //BGMの音量調整
    public void SetBGMVolume()
    {
        bgmAudioSource.volume = bgmVolumeSlider.value;
        PlayerPrefs.SetFloat("BGMVolume", bgmVolumeSlider.value); //音量を保存
    }
    //SEの音量調整
    public void SetSEVolume()
    {
        seAudioSource.volume = seVolumeSlider.value;
        PlayerPrefs.SetFloat("SEVolume", seAudioSource.volume); //音量を保存
    }
}
