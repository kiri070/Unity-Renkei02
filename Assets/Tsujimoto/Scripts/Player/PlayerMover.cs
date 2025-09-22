using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class PlayerMover : MonoBehaviour
{
    [Header("プレイヤーID")] [Tooltip("1~2")] public int playerIndex;
    Rigidbody rb;
    Vector3 move; //移動するためのベクトル
    PlayerCnt playerCnt;
    GameManager gameManager;
    // 持っているオブジェクトを記録
    [HideInInspector] public Rigidbody heldObject;

    [Header("物体を持てる範囲")] [SerializeField] Vector3 boxSize = new Vector3(1f, 0.2f, 1f);
    [SerializeField] Vector3 offSet = new Vector3(0f, 0f, 0f);
    [Header("物体のレイヤー")] public LayerMask objLayer;
    [HideInInspector]
    public float jumpForce; //ジャンプ力
    [HideInInspector]
    public bool jumping; //ジャンプをするか
    [HideInInspector]
    public bool canJump; //ジャンプが可能か
    bool touchDeathArea = false; //DeathAreaに触れたか
    bool moving = false; //移動入力があるかどうか
    Collider collider;

    Vector3 originalScale; //プレイヤーの大きさ

    [Header("各カメラを格納")]
    public Camera gameCamera;
    CameraCnt cameraCnt;

    [Header("各プレイヤーのオブジェクトを格納")]
    public GameObject player;

    // [Header("マテリアル")]
    // public Material hideMaterial;

    [Header("エフェクト")]
    [Tooltip("敵と衝突")] public GameObject nockBackEffect;
    [Tooltip("ジャンプ")] public GameObject jumpEffect;
    [Tooltip("氷の床")] public GameObject frozenEffect;
    [Tooltip("トランポリン")] public GameObject tranpolineEffect;

    Renderer[] renderers; // 複数のRenderer（子オブジェクト含む）を管理
    List<Material[]> defaultMaterials = new List<Material[]>(); // 各Rendererの初期マテリアルを保存

    //氷関連
    bool onIce; //氷の上にいるか
    Vector3 slideVelocity; //滑り中の速度
    float frozenEffectTime; //エフェクトの生成時間
    Queue<GameObject> effectPool = new Queue<GameObject>(); //キュー

    [HideInInspector] public bool canMove = true; //動けるかどうか
    [HideInInspector] public bool useTrampoline = false; //トランポリンを使用中かどうか
    [HideInInspector] public bool onRoof = false; //天井にいるかどうか

    [Header("敵衝突時のノックバック:水平方向")][SerializeField] float nockBack_Horizontal = 30f;
    [Header("敵衝突時のノックバック:垂直方向")][SerializeField] float nockBack_Vertical = 10f;
    [Header("ノックバック時の操作不能から回復する時間")]public float recoveryKnockbackTime = 0.7f; //ノックバックの操作の操作不能から回復する時間

    SoundManager soundManager;
    SoundsList soundsList;

    //こるーちん
    Coroutine gimicBlock_coroutine = null; //ギミックブロック

    [HideInInspector] public bool isZoomOutPos = false; //カメラのズームアウトエリアにいるかどうか
    [HideInInspector] public bool isHighCameraPos = false; //カメラのy値を高く設定するエリアにいるかどうか
    [HideInInspector] public bool isBeltconveyorPos = false; //ベルトコンベアの近くにいるかどうか
    [HideInInspector] public ZoomOutSetting zoomOutSetting; //ズームアウトの設定
    public bool isZoomoutPos { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCnt = GameObject.FindObjectOfType<PlayerCnt>();
        soundManager = FindObjectOfType<SoundManager>();
        soundsList = FindObjectOfType<SoundsList>();
        cameraCnt = FindObjectOfType<CameraCnt>();
        originalScale = transform.localScale; //初期の大きさを保存
        gameManager = FindObjectOfType<GameManager>();
        collider = GetComponent<CapsuleCollider>();

        // 子を含むRendererをすべて取得
        renderers = GetComponentsInChildren<Renderer>();

        // 初期マテリアルを保存(hidematerial)
        // foreach (Renderer r in renderers)
        // {
        //     // マテリアルのコピーを保存
        //     Material[] mats = r.materials;
        //     Material[] copied = new Material[mats.Length];
        //     for (int i = 0; i < mats.Length; i++)
        //     {
        //         copied[i] = mats[i];
        //     }
        //     defaultMaterials.Add(copied);
        // }
    }

    //移動ベクトルをこのキャラのベクトルに代入
    public void Assignment(Vector3 input)
    {
        move = input;
    }

    void Update()
    {
        //移動入力があるかどうか
        if (move.magnitude > 0.01) moving = true;
        else moving = false;

        // OffScreen();
        // WallChecker();
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

        BringObj bringObj = FindObjectOfType<BringObj>();

        //オブジェクトが範囲内かつ、持つボタンを押したら
        if (hits.Length > 0 && isBring)
        {
            //宝箱を運んでいる時は運搬中フラグを立てる
            if (playerIndex == 1)
            {
                bringObj.player1_isBringing = true;
            }
            else
            {
                bringObj.player2_isBringing = true;
            }

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
                //通常時
                if (!playerCnt.OnUnder_OverGimic)
                {
                    Vector3 targetPos = transform.position + transform.forward * 1.5f + Vector3.up * 1f;
                    heldObject.MovePosition(targetPos);
                }
                //上下ギミック時
                else if (playerCnt.OnUnder_OverGimic)
                {
                    //天井の場合,少し下で運ぶ
                    if (bringObj.top)
                    {
                        Vector3 targetPos = transform.position + transform.forward * 1.5f + Vector3.down * 1f;
                        heldObject.MovePosition(targetPos);
                    }
                    //地面の場合,少し上で運ぶ
                    else if (bringObj.bottom)
                    {
                        Vector3 targetPos = transform.position + transform.forward * 1.5f + Vector3.up * 1f;
                        heldObject.MovePosition(targetPos);
                    }

                }
            }
        }
        else if (!isBring)
        {
            if (heldObject != null)
            {
                //宝箱を運んでいない時は運搬中フラグをオフ
                if (bringObj != null)
                {
                    if (playerIndex == 1)
                    {
                        bringObj.player1_isBringing = false;
                    }
                    else
                    {
                        bringObj.player2_isBringing = false;
                    }
                }

                //通常時
                if (!playerCnt.OnUnder_OverGimic)
                {
                    Collider obj_Col = heldObject.GetComponent<Collider>();
                    if (obj_Col != null) obj_Col.isTrigger = false;
                    heldObject.useGravity = true;
                    heldObject = null;
                }
                //上下ギミック時
                else if (playerCnt.OnUnder_OverGimic)
                {
                    //天井の場合,少し下でおろす
                    if (bringObj.top)
                    {
                        Collider obj_Col = heldObject.GetComponent<Collider>();
                        Vector3 targetPos = transform.position + transform.forward * 1.5f + Vector3.down * 1f;
                        heldObject.MovePosition(targetPos);
                        if (obj_Col != null) obj_Col.isTrigger = false;
                        heldObject.useGravity = true;
                        heldObject = null;
                    }
                    //地面の場合,少し上でおろす
                    else if (bringObj.bottom)
                    {
                        Collider obj_Col = heldObject.GetComponent<Collider>();
                        if (obj_Col != null) obj_Col.isTrigger = false;
                        heldObject.useGravity = true;
                        heldObject = null;
                    }

                }
            }
        }
    }

    //移動処理
    void Move()
    {
        //傾斜を確認
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            //上方向の傾斜を取得
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (hit.collider.gameObject.tag == "Floor")
            {
                //30度以上かつ、静止している場合
                if (angle > 30f && !moving)
                {
                    //摩擦を追加
                    collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
                    collider.material.staticFriction = 1;
                }
                else
                {
                    //摩擦を削除
                    collider.material.frictionCombine = PhysicMaterialCombine.Multiply;
                    collider.material.staticFriction = 0;
                }
            }
        }



        //上下ギミック起動中、天井のキャラは擬似的に重力かける
            if (playerCnt.OnUnder_OverGimic && onRoof)
            {
                rb.useGravity = false; //重力をオフ
                rb.AddForce(Vector3.up * 100f, ForceMode.Acceleration); //擬似的な重力を上方向に作る
            }
            else if (!playerCnt.OnUnder_OverGimic)
            {
                rb.useGravity = true; //重力をオン
            }

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
        //向き変更(移動中のみ)
        if (move.magnitude > 0.1f)
        {
            //カスタムベクトルを設定
            Vector3 customUp = (playerCnt.OnUnder_OverGimic && onRoof) ? Vector3.down : Vector3.up;
            Quaternion targetRotation = Quaternion.LookRotation(move, customUp);

            // 天井に張り付いているプレイヤーはZ軸180度を維持した回転に変換
            if (onRoof && playerCnt.OnUnder_OverGimic)
            {
                // Y軸とX軸だけをSlerpで変化、Zは180に固定する
                Vector3 euler = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f).eulerAngles;
                transform.rotation = Quaternion.Euler(euler.x, euler.y, 180f);
            }
            else
            {
                // 通常の回転
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        //ジャンプ
        if (!playerCnt.OnUnder_OverGimic)
        {
            if (jumping && !useTrampoline)
            {
                rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
                jumping = false;
                canJump = false;
            }
        }
        //上下ギミック中
        else if (playerCnt.OnUnder_OverGimic)
        {
            //地面にいる時
            if (jumping && !useTrampoline && !onRoof)
            {
                rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
                jumping = false;
                canJump = false;
            }
            //天井にいる時
            else if (jumping && !useTrampoline && onRoof)
            {
                rb.AddForce(0f, -jumpForce, 0f, ForceMode.Impulse);
                jumping = false;
                canJump = false;
            }
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

    // //壁などに隠れている時
    // void WallChecker()
    // {
    //     Vector3 camPos = gameCamera.transform.position;
    //     Vector3 targetPos = player.transform.position;
    //     Vector3 direction = targetPos - camPos;

    //     //カメラからプレイヤーまでの直線上にRayを飛ばす
    //     if (Physics.Raycast(camPos, direction.normalized, out RaycastHit hit, direction.magnitude))
    //     {
    //         //隠れていたら
    //         if (hit.collider.gameObject != player)
    //         {
    //             if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor"))
    //             {
    //                 SetAllMaterialsToHide(); //Hideマテリアルに変更
    //             }
    //         }
    //         else if (hit.collider.gameObject == player)
    //         {
    //             RestoreAllMaterials(); //デフォルトマテリアルに変更
    //         }
    //     }

    // }
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
        //トランポリン使用後に地面に触れたら
        if (other.gameObject.CompareTag("Floor") && useTrampoline) useTrampoline = false; //トランポリン使用中フラグをオフ
        //トランポリンに触れたら
        if (other.gameObject.CompareTag("Bound"))
        {
            rb.AddForce(0f, 50f, 0f, ForceMode.Impulse); //ジャンプさせる
            Instantiate(tranpolineEffect, transform.position, tranpolineEffect.transform.rotation); //エフェクトを生成
            useTrampoline = true; //トランポリン使用中フラグを立てる
        }
        //敵に触れたら
        if (other.gameObject.CompareTag("Enemy"))
        {
            //無敵状態なら処理をスキップ
            if (playerCnt.invincible) return;

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

                // 動くオブジェクトをリセットする
                foreach (TriggeredMovingPlatform platform in FindObjectsOfType<TriggeredMovingPlatform>())
                {
                    Debug.Log("Reset対象: " + platform.name);
                    platform.ResetPlatform();
                }
            }
            else
            {
                touchDeathArea = true;
                //タイマー減少
                gameManager.DecreaseTimer(gameManager.decreaseFallTimer);
                playerCnt.SpawnCheckPoint();

                // 動くオブジェクトをリセットする
                foreach (TriggeredMovingPlatform platform in FindObjectsOfType<TriggeredMovingPlatform>())
                {
                    Debug.Log("Reset対象: " + platform.name);
                    platform.ResetPlatform();
                }
            }


            // GameOverManager.becauseGameOver = "落下してしまった!!"; //死因
            // soundManager.OnPlaySE(soundsList.explosionSE);
            // StartCoroutine(DelayLoadScene2(1.5f)); //遅延してシーン変遷
            // GameManager.ToGameOverState();
        }
        //画面外判定なら
        if (other.CompareTag("GameOverWall") && !playerCnt.invincible)
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

                //スタート地点に戻る
                playerCnt.SpawnCheckPoint();
            }


            // GameOverManager.becauseGameOver = "落下してしまった!!"; //死因
            // soundManager.OnPlaySE(soundsList.explosionSE);
            // StartCoroutine(DelayLoadScene2(1.5f)); //遅延してシーン変遷
            // GameManager.ToGameOverState();
        }
        //上下ギミック中の画面外判定なら
        if (other.CompareTag("GameOverWall_Gimic") && !playerCnt.invincible)
        {
            soundManager.OnPlaySE(soundsList.fallSE); //SE
            touchDeathArea = true;
            //タイマー減少
            gameManager.DecreaseTimer(gameManager.decreaseFallTimer);

            //スタート地点に戻る
            if (!playerCnt.currentCheckPoint_TopBottom) playerCnt.SpwanStartPoint_Gimic(); //チェックポイントがない場合
            else if (playerCnt.currentCheckPoint_TopBottom) playerCnt.SpwanCheckPoint_Gimic(); //チェックポイントがある場合
        }
        //魔法に当たったら
        if (other.CompareTag("Majic"))
        {
            //無敵状態なら処理をスキップ
            if (playerCnt.invincible) return;

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
        }
        //Bombに触れたら
        if (other.CompareTag("Bomb"))
        {
            StartCoroutine(cameraCnt.ShakeCamera(0.7f, 0.3f)); //カメラを揺らす
            canMove = false;
            StartCoroutine(RecoveryKnockback(recoveryKnockbackTime));
        }
        //GimicBlock_Bombに触れたら
        if (other.CompareTag("GimicBlock_Bomb"))
        {
            GameObject gimicBlock = GameObject.Find("GimicBlock"); 
            if (gimicBlock != null)
            {
                if (gimicBlock_coroutine != null)
                {
                    StopCoroutine(gimicBlock_coroutine);
                }
                // 毎回新しくスタート
                gimicBlock_coroutine = StartCoroutine(DelayActive_GameObject(gimicBlock, 2));
            }
            canMove = false;
            StartCoroutine(RecoveryKnockback(recoveryKnockbackTime));
        }
        //チェックポイントに触れたら
        if (other.CompareTag("CheckPoint"))
        {
            CheckPointManager cpManager = other.GetComponent<CheckPointManager>();
            //チェックポイントのスクリプトのアクティブ状況を確認
            if (cpManager != null && !cpManager.isActive)
            {
                cpManager.isActive = true;

                //同じチェックポイントなら処理をスキップ
                if (playerCnt.currentCheckPoint == other.gameObject) return;

                //最新チェックポイントを保存
                playerCnt.currentCheckPoint = other.gameObject;
                //チェックポイントのエフェクトを再生
                playerCnt.currentCheckPoint.transform.Find("CheckPointEffect").gameObject.SetActive(true);
                //効果音
                soundManager.OnPlaySE(soundsList.checkPointSE);
            }
        }
        //チェックポイントに触れたら
        if (other.CompareTag("CheckPoint_TopBottom"))
        {
            CheckPointManager cpManager = other.GetComponent<CheckPointManager>();
            //チェックポイントのスクリプトのアクティブ状況を確認
            if (cpManager != null && !cpManager.isActive)
            {
                cpManager.isActive = true;

                //同じチェックポイントなら処理をスキップ
                if (playerCnt.currentCheckPoint == other.gameObject) return;

                //最新チェックポイントを保存
                playerCnt.currentCheckPoint_TopBottom = other.gameObject;
                //チェックポイントのエフェクトを再生
                playerCnt.currentCheckPoint_TopBottom.transform.Find("CheckPointEffect").gameObject.SetActive(true);
                //効果音
                soundManager.OnPlaySE(soundsList.checkPointSE);
            }
        }
        //大砲の球に触れたら
        if (other.CompareTag("CannonBall"))
        {
            if (!playerCnt.invincible)
            {
                canMove = false;
                StartCoroutine(RecoveryKnockback(recoveryKnockbackTime));
                StartCoroutine(cameraCnt.ShakeCamera(0.7f, 1f)); //カメラを揺らす
            }

        }
        //お宝回復に触れたら
        if (other.CompareTag("Treasure"))
        {
            //お宝の価値をプラス
            gameManager.PlusBoxValue(10);
            Destroy(other.gameObject);
        }
        //カメラのズームアウトエリアにいたら
        if (other.CompareTag("ZoomOutPos"))
        {
            isZoomOutPos = true;
        }
        //カメラのy値を高く設定するエリア
        if (other.CompareTag("HighCameraArea"))
        {
            isHighCameraPos = true;
        }
        //ズームアウトエリアなら
        if (other.CompareTag("ZoomOutObj"))
        {
            zoomOutSetting = other.gameObject.GetComponent<ZoomOutSetting>();
            if (zoomOutSetting != null)
            {
                zoomOutSetting = other.GetComponent<ZoomOutSetting>();
                isZoomoutPos = true;
            }
        }
    }
    //オブジェクトのmeshを切り替える
    IEnumerator DelayActive_GameObject(GameObject gameObject, int delay)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(delay); //遅延
        gameObject.GetComponent<MeshRenderer>().enabled = true;
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
        //カメラのズームアウトエリアから出たら
        if(other.CompareTag("ZoomOutPos"))
        {
            isZoomOutPos = false;
        }
        //カメラのy値を高く設定するエリア
        if (other.CompareTag("HighCameraArea"))
        {
            isHighCameraPos = false;
        }
        //ズームアウトエリアなら
        if (other.CompareTag("ZoomOutObj"))
        {
            if (other.CompareTag("ZoomOutObj"))
            {
                if (zoomOutSetting != null && other.gameObject == zoomOutSetting.gameObject)
                {
                    zoomOutSetting = null;
                }
                isZoomOutPos = false;
            }
        }
    }

    public static void GameClear()
    {
        GameManager.ToClearState();
        Debug.Log("クリア");
    }

    // //Hideマテリアルに変更する関数
    // void SetAllMaterialsToHide()
    // {
    //     foreach (Renderer r in renderers)
    //     {
    //         Material[] newMats = new Material[r.materials.Length];
    //         for (int i = 0; i < newMats.Length; i++)
    //         {
    //             newMats[i] = hideMaterial;
    //         }
    //         r.materials = newMats;
    //     }
    // }

    //初期のマテリアルに戻す関数
    void RestoreAllMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = defaultMaterials[i];
        }
    }

    //ノックバック時間を計測
    public IEnumerator RecoveryKnockback(float time)
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
