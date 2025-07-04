using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class PlayerMover : MonoBehaviour
{
    [Header("プレイヤーID")][Tooltip("1~2")] public int playerIndex;
    Rigidbody rb;
    Vector3 move; //移動するためのベクトル
    PlayerCnt playerCnt;
    GameManager gameManager;
    // 持っているオブジェクトを記録
    Rigidbody heldObject;

    [Header("物体を持てる範囲")][SerializeField] Vector3 boxSize = new Vector3(1f, 0.2f, 1f);
    [SerializeField] Vector3 offSet = new Vector3(0f, 0f, 0f);
    [Header("物体のレイヤー")] public LayerMask objLayer;
    [HideInInspector]
    public float jumpForce; //ジャンプ力
    [HideInInspector]
    public bool jumping; //ジャンプをするか
    [HideInInspector]
    public bool canJump; //ジャンプが可能か
    bool touchDeathArea = false; //DeathAreaに触れたか

    Vector3 originalScale; //プレイヤーの大きさ

    [Header("各カメラを格納")]
    public Camera gameCamera;
    CameraCnt cameraCnt;

    [Header("各プレイヤーのオブジェクトを格納")]
    public GameObject player;

    [Header("マテリアル")]
    public Material hideMaterial;

    [Header("エフェクト")]
    [Tooltip("敵と衝突")] public GameObject nockBackEffect;
    [Tooltip("ジャンプ")] public GameObject jumpEffect;
    [Tooltip("氷の床")] public GameObject frozenEffect;

    Renderer[] renderers; // 複数のRenderer（子オブジェクト含む）を管理
    List<Material[]> defaultMaterials = new List<Material[]>(); // 各Rendererの初期マテリアルを保存

    //氷関連
    bool onIce; //氷の上にいるか
    Vector3 slideVelocity; //滑り中の速度
    float frozenEffectTime; //エフェクトの生成時間
    Queue<GameObject> effectPool = new Queue<GameObject>(); //キュー

    bool canMove = true; //動けるかどうか
    
    [Header("敵衝突時のノックバック:水平方向")][SerializeField] float nockBack_Horizontal = 30f;
    [Header("敵衝突時のノックバック:垂直方向")][SerializeField] float nockBack_Vertical = 10f;
    [Header("ノックバック時の操作不能から回復する時間")][SerializeField] float recoveryKnockbackTime = 0.7f; //ノックバックの操作の操作不能から回復する時間

    SoundManager soundManager;
    SoundsList soundsList;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCnt = GameObject.FindObjectOfType<PlayerCnt>();
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
        cameraCnt = FindObjectOfType<CameraCnt>();
        originalScale = transform.localScale; //初期の大きさを保存
        gameManager = FindObjectOfType<GameManager>();

        // 子を含むRendererをすべて取得
        renderers = GetComponentsInChildren<Renderer>();

        // 初期マテリアルを保存
        foreach (Renderer r in renderers)
        {
            // マテリアルのコピーを保存
            Material[] mats = r.materials;
            Material[] copied = new Material[mats.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                copied[i] = mats[i];
            }
            defaultMaterials.Add(copied);
        }
    }

    //移動ベクトルをこのキャラのベクトルに代入
    public void Assignment(Vector3 input)
    {
        move = input;
    }

    void Update()
    {
        // OffScreen();
        WallChecker();
    }

    void FixedUpdate()
    {
        BringArea();
        if (canMove)
            Move();
    }

    //物体を持てる範囲
    void BringArea()
    {
        //playerIndexが1なら左,それ以外なら右
        bool isBring = (playerIndex == 1) ? playerCnt.isPlayer1BringObj : playerCnt.isPlayer2BringObj;
        // プレイヤーの前方＋ちょい上
        Vector3 center = transform.position + transform.forward * 1.5f + Vector3.up * 1f;

        // 物体を検索
        Collider[] hits = Physics.OverlapBox(center, boxSize / 2, Quaternion.identity, objLayer);

        //オブジェクトが範囲内かつ、持つボタンを押したら
        if (hits.Length > 0 && isBring)
        {
            heldObject = hits[0].attachedRigidbody;
            if (heldObject != null)
            {
                Collider obj_Col = hits[0];
                obj_Col.isTrigger = true;

                heldObject.useGravity = false;
                heldObject.velocity = Vector3.zero; // 落ちてる途中なら停止
            }
            // 既に持っている場合は、位置を前方に維持
            if (heldObject != null && isBring)
            {
                Vector3 targetPos = transform.position + transform.forward * 1.5f + Vector3.up * 1f;
                heldObject.MovePosition(targetPos);
            }
        }
        else if (!isBring)
        {
            if (heldObject != null)
            {
                Collider obj_Col = heldObject.GetComponent<Collider>();
                if (obj_Col != null) obj_Col.isTrigger = false;
                heldObject.useGravity = true;
                heldObject = null;
            }
        }
    }

    //移動処理
    void Move()
    {
        //氷の上の場合
        if (onIce)
        {
            //入力がある場合
            if (move.magnitude > 0.1f)
            {
                slideVelocity = move;
            }
            //入力がない場合
            else
            {
                slideVelocity *= 0.96f; //少し滑らせる
            }

            rb.velocity = new Vector3(slideVelocity.x, rb.velocity.y, slideVelocity.z);
        }
        //通常の移動
        else
        {
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        }

        // 向き変更（移動中のみ）
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }

        //ジャンプ
        if (jumping)
        {
            rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
            jumping = false;
            canJump = false;
        }
    }

    //スライディング開始:大きさを小さく
    public void StartSliding()
    {
        transform.localScale = originalScale * 0.5f;
    }
    //スライディング終了:大きさを戻す
    public void EndSliding()
    {
        transform.localScale = originalScale;
    }

    //画面外検知処理(保留)
    void OffScreen()
    {
        //このオブジェクトをカメラの画面上での位置に変換
        Vector3 viewPos = gameCamera.WorldToViewportPoint(transform.position);

        //画面外に出た時
        if (viewPos.x < 0 || viewPos.x > 1 ||
           viewPos.y < 0 || viewPos.y > 1 ||
           viewPos.z < 0)
        {

            if (canJump && !touchDeathArea)
            {
                if (!playerCnt.currentCheckPoint)
                {
                    //タイマー減少
                    gameManager.DecreaseTimer(gameManager.decreaseFallTimer);

                    //スタート地点に戻る
                    playerCnt.SpwanStartPoint();
                }
                //チェックポイントがあったら
                else
                {
                    //タイマー減少
                    gameManager.DecreaseTimer(gameManager.decreaseFallTimer);

                    playerCnt.SpawnCheckPoint();
                }

            }

            //ジャンプしていないとき
            // if (canJump && !touchDeathArea)
            // {
            //     GameOverManager.becauseGameOver = "画面外に出てしまった!!"; //死因
            //     GameManager.ToGameOverState();
            // }
        }
    }

    //壁などに隠れている時
    void WallChecker()
    {
        Vector3 camPos = gameCamera.transform.position;
        Vector3 targetPos = player.transform.position;
        Vector3 direction = targetPos - camPos;

        //カメラからプレイヤーまでの直線上にRayを飛ばす
        if (Physics.Raycast(camPos, direction.normalized, out RaycastHit hit, direction.magnitude))
        {
            //隠れていたら
            if (hit.collider.gameObject != player)
            {
                if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor"))
                {
                    SetAllMaterialsToHide(); //Hideマテリアルに変更
                }
            }
            else if (hit.collider.gameObject == player)
            {
                RestoreAllMaterials(); //デフォルトマテリアルに変更
            }
        }

    }
    //CollisionEnter
    void OnCollisionEnter(Collision other)
    {
        //地面に触れたら
        if (other.gameObject.CompareTag("Floor") && !canJump)
        {
            canJump = true;
            touchDeathArea = false;
            Instantiate(jumpEffect, transform.position, Quaternion.identity);
        }
        //敵に触れたら
        if (other.gameObject.CompareTag("Enemy"))
        {
            //効果音再生
            soundManager.OnPlaySE(soundsList.nockBackSE);

            //エフェクト再生
            Instantiate(nockBackEffect, transform.position, Quaternion.identity);

            canMove = false;
            StartCoroutine(RecoveryKnockback(recoveryKnockbackTime));

            //水平方向の力
            Vector3 horizontal = (transform.position - other.transform.position).normalized;
            horizontal.y = 0f;
            horizontal = horizontal * nockBack_Horizontal;

            // 上方向の力
            Vector3 vertical = Vector3.up * nockBack_Vertical; // ジャンプの高さ

            //二つをまとめる
            Vector3 nockBack = horizontal + vertical;


            //ノックバック
            rb.AddForce(nockBack, ForceMode.Impulse);
            // GameOverManager.becauseGameOver = "敵に接触してしまった!!"; //死因
            // GameManager.ToGameOverState();
        }
    }

    //TriggerEnter
    void OnTriggerEnter(Collider other)
    {
        //落下したら
        if (other.CompareTag("DeathArea"))
        {
            soundManager.OnPlaySE(soundsList.fallSE); //SE
            if (!playerCnt.currentCheckPoint)
            {
                touchDeathArea = true;
                //タイマー減少
                gameManager.DecreaseTimer(gameManager.decreaseFallTimer);

                //スタート地点に戻る
                playerCnt.SpwanStartPoint();
            }
            else
            {
                touchDeathArea = true;
                //タイマー減少
                gameManager.DecreaseTimer(gameManager.decreaseFallTimer);
                playerCnt.SpawnCheckPoint();
            }


            // GameOverManager.becauseGameOver = "落下してしまった!!"; //死因
            // soundManager.OnPlaySE(soundsList.explosionSE);
            // StartCoroutine(DelayLoadScene2(1.5f)); //遅延してシーン変遷
            // GameManager.ToGameOverState();
        }
        //魔法に当たったら
        if (other.CompareTag("Majic"))
        {
            //無敵時間ではなかったら
            if (!playerCnt.invincible)
            {
                //チェックポイントがない場合
                if (!playerCnt.currentCheckPoint)
                {
                    //タイマー減少
                    gameManager.DecreaseTimer(gameManager.decreaseMimicTimer);

                    //スタート地点に戻る
                    playerCnt.SpwanStartPoint();
                }
                //チェックポイントがある場合
                else
                {
                    //タイマー減少
                    gameManager.DecreaseTimer(gameManager.decreaseMimicTimer);
                    playerCnt.SpawnCheckPoint();
                }

            }
            // GameOverManager.becauseGameOver = "魔法で黒焦げにされた..."; //死因
            // GameManager.ToGameOverState();
        }
        //Bombに触れたら
        if (other.CompareTag("Bomb"))
        {
            StartCoroutine(cameraCnt.ShakeCamera(0.7f, 0.3f)); //カメラを揺らす
            canMove = false;
            StartCoroutine(RecoveryKnockback(recoveryKnockbackTime));
        }
        //チェックポイントに触れたら
        if (other.CompareTag("CheckPoint"))
        {
            //最新チェックポイントを保存
            playerCnt.currentCheckPoint = other.gameObject;
            //チェックポイントのエフェクトを再生
            playerCnt.currentCheckPoint.transform.Find("CheckPointEffect").gameObject.SetActive(true);
        }
        //大砲の球に触れたら
        if (other.CompareTag("CannonBall"))
        {
            canMove = false;
            StartCoroutine(RecoveryKnockback(recoveryKnockbackTime));
            StartCoroutine(cameraCnt.ShakeCamera(0.7f, 1f)); //カメラを揺らす
        }
        //お宝回復に触れたら
        if (other.CompareTag("Treasure"))
        {
            //お宝の価値をプラス
            gameManager.PlusBoxValue(10);
            Destroy(other.gameObject);
        }
    }

    //遅延させてゲームオーバーシーンに変遷
    IEnumerator DelayLoadScene(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);
        GameManager.ToGameOverState();
    }
    //時間時程してゲームオーバーシーンに変遷
    IEnumerator DelayLoadScene2(float time)
    {
        yield return new WaitForSeconds(time);
        GameManager.ToGameOverState();
    }

    //OnTriggerStay
    void OnTriggerStay(Collider other)
    {
        //氷に触れている場合
        if (other.CompareTag("FrozenArea"))
        {
            onIce = true;
            //エフェクト
            frozenEffectTime += Time.deltaTime;
            if (frozenEffectTime > 0.01f)
            {
                GameObject effect = Instantiate(frozenEffect, transform.position, Quaternion.identity);
                effectPool.Enqueue(effect); //キューに入れる
                if (effectPool.Count > 30)
                {
                    GameObject old = effectPool.Dequeue(); //古いものを取り出す
                    Destroy(old);
                }
                frozenEffectTime = 0f;
            }
        }
    }

    //OnTriggerExit
    void OnTriggerExit(Collider other)
    {
        //氷から出たら
        if (other.CompareTag("FrozenArea"))
        {
            onIce = false;
            slideVelocity = Vector3.zero; //滑りをリセット
        }
    }

    public static void GameClear()
    {
        GameManager.ToClearState();
        Debug.Log("クリア");
    }

    //Hideマテリアルに変更する関数
    void SetAllMaterialsToHide()
    {
        foreach (Renderer r in renderers)
        {
            Material[] newMats = new Material[r.materials.Length];
            for (int i = 0; i < newMats.Length; i++)
            {
                newMats[i] = hideMaterial;
            }
            r.materials = newMats;
        }
    }

    //初期のマテリアルに戻す関数
    void RestoreAllMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = defaultMaterials[i];
        }
    }

    //ノックバック時間を計測
    IEnumerator RecoveryKnockback(float time)
    {
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    //敵側から呼び出して踏みつけ音を鳴らす関数
    public void OnStepEnemy()
    {
        soundManager.OnPlaySE(soundsList.stepOnPlayer);
    }

    //物体を持てる範囲を描画
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + Vector3.up * 1f + offSet;
        Gizmos.DrawWireCube(center, boxSize);
    }
}
