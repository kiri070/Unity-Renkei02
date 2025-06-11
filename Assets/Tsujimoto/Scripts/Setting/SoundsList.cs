using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class SoundsList : MonoBehaviour
{
    [Header("SE")]
    [Header("プレイヤー関連")]
    [Tooltip("効果音:ジャンプ")] public AudioClip jumpSE;
    [Tooltip("効果音:球を打つ")] public AudioClip shotSE;
    [Tooltip("効果音:球が当たる")] public AudioClip hitShotSE;
    [Tooltip("効果音:ノックバック")] public AudioClip nockBackSE;

    [Header("ギミック関連")]
    [Tooltip("効果音:ボムの爆発")] public AudioClip explosionSE;

    [Header("敵関連")]
    [Tooltip("効果音:ミミックの魔法")] public AudioClip mimicMagicSE;
    [Tooltip("効果音:敵の死亡")] public AudioClip killEnemySE;
    [Tooltip("効果音:敵のジャンプ攻撃")] public AudioClip jumpAttack;

    [Header("システム関連")]
    [Tooltip("効果音:設定画面を開く")] public AudioClip openSetting;
    [Tooltip("効果音:ボタンをクリック")] public AudioClip clickButton;

    [Header("BGM")]
    [Tooltip("BGM:ゲームの音楽")]
    public AudioClip gameBGM;
}
