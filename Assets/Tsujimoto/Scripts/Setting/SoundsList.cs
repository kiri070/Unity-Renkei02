using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsList : MonoBehaviour
{
    [Header("SE")]
    [Header("プレイヤー関連")]
    [Tooltip("効果音:ジャンプ")] public AudioClip jumpSE;
    [Tooltip("効果音:ノックバック")] public AudioClip nockBackSE;
    [Tooltip("効果音:落下")] public AudioClip fallSE;

    [Header("その他")]
    [Tooltip("効果音:ゲームオーバー")] public AudioClip gameoverSE;
    [Tooltip("効果音:ゲームクリア")] public AudioClip gameclearSE;

    [Header("ギミック関連")]
    [Tooltip("効果音:ボムの爆発")] public AudioClip explosionSE;
    [Tooltip("効果音:大砲発射")] public AudioClip shotCannonSE;
    [Tooltip("効果音:宝箱がダメージを受ける")] public AudioClip treasureDamagedSE;
    [Tooltip("効果音:チェックポイント")] public AudioClip checkPointSE;
    [Tooltip("効果音:ゴールに触れる")] public AudioClip touchGoalSE;

    [Header("敵関連")]
    [Tooltip("効果音:ミミックの魔法")] public AudioClip mimicMagicSE;
    [Tooltip("効果音:敵の死亡")] public AudioClip killEnemySE;
    [Tooltip("効果音:敵のジャンプ攻撃")] public AudioClip jumpAttack;
    [Tooltip("効果音:踏まれた時")] public AudioClip stepOnPlayer;

    [Header("システム関連")]
    [Tooltip("効果音:設定画面を開く")] public AudioClip openSetting;
    [Tooltip("効果音:ボタンをクリック")] public AudioClip clickButton;
    [Tooltip("効果音:ステージセレクトボタン")] public AudioClip clickStage;

    [Header("BGM")]
    [Tooltip("BGM:ゲームの音楽")]
    public AudioClip tittleBGM;
    public AudioClip stageSelectBGM;
    public AudioClip stage1BGM;
}
