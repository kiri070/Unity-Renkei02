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
    float lastPlayedTime = 0f; //前回の音から何秒たったかを記録
    float minInterval = 0.05f; //音の重複を消す(指定の秒数に一回まで)
    void Start()
    {
        //コンポーネント取得
        soundsList = GetComponent<SoundsList>();

        //BGM,SEの音量を取り出し
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        seVolumeSlider.value = PlayerPrefs.GetFloat("SEVolume");
    }

    /// <summary>
    /// 効果音(SE)を再生します。
    /// </summary>
    /// <param name="audioClip">再生する効果音のAudioClip</param>
    /// <param name="volume">再生する音量 (0.0～1.0)。既定値は 1.0 です。</param>
    public void OnPlaySE(AudioClip audioClip, float volume = 1f)
    {
        if (audioClip == null)
            return;
        seAudioSource.PlayOneShot(audioClip, volume);
    }

    /// <summary>
    /// 音楽(BGM)を再生します。
    /// </summary>
    /// <param name="audioClip">再生する音楽のAudioClip</param>
    public void OnPlayBGM(AudioClip audioClip)
    {
        //BGMが複数再生されないように
        if (bgmAudioSource.isPlaying)
            bgmAudioSource.Stop();

        bgmAudioSource.clip = audioClip;
        bgmAudioSource.Play();
    }

    /// <summary>
    /// BGMの音量調整
    /// </summary>
    public void SetBGMVolume()
    {
        bgmAudioSource.volume = bgmVolumeSlider.value;
        PlayerPrefs.SetFloat("BGMVolume", bgmVolumeSlider.value); //音量を保存
    }
    /// <summary>
    /// SEの音量調整
    /// </summary>
    public void SetSEVolume()
    {
        seAudioSource.volume = seVolumeSlider.value;
        PlayerPrefs.SetFloat("SEVolume", seAudioSource.volume); //音量を保存
    }
}
