using JetBrains.Annotations;
using UnityEngine;

public class PlayerCnt : MonoBehaviour
{
    [Header("プレイヤー格納")]
    public PlayerMover mover1;
    public PlayerMover mover2;

    [Header("移動速度")]
    [Range(1f, 10f)]
    [SerializeField]
    private float moveSpeed = 5f; //移動速度

    [Header("ジャンプ力")]
    [Range(1f, 50f)]
    [SerializeField]
    private float jumpForce; //ジャンプ力

    void Start()
    {
        
    }

    void Update()
    {
        // Player1: WASD
        float x1 = Input.GetAxis("Horizontal");
        float z1 = Input.GetAxis("Vertical");
        Vector3 input1 = new Vector3(x1, 0, z1) * moveSpeed;
        mover1.Assignment(input1);

        // Player2: IJKL
        float x2 = 0, z2 = 0;
        if (Input.GetKey(KeyCode.J)) x2 = -1;
        if (Input.GetKey(KeyCode.L)) x2 = 1;
        if (Input.GetKey(KeyCode.I)) z2 = 1;
        if (Input.GetKey(KeyCode.K)) z2 = -1;
        Vector3 input2 = new Vector3(x2, 0, z2) * moveSpeed;
        mover2.Assignment(input2);

        //ジャンプ(共通)
        if (Input.GetKey(KeyCode.Space) && mover1.canJump && mover2.canJump)
        {
            mover1.jumpForce = this.jumpForce;
            mover1.jumping = true;

            mover2.jumpForce = this.jumpForce;
            mover2.jumping = true;
        }
    }
}
