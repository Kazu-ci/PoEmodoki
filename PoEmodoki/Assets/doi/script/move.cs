using UnityEngine;

public class move : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpPower = 8f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    // ★ キャラの向きを固定する値
    private Quaternion fixedRotation;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // ★ 初期向きを固定して記録
        fixedRotation = transform.rotation;
    }

    void Update()
    {
        Move();

        // ★ 回転を完全に固定（最重要）
        transform.rotation = fixedRotation;
    }

    void Move()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // ★ カメラ基準の移動方向
        Transform cam = Camera.main.transform;
        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;

        Vector3 move = camForward * z + camRight * x;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // ★ ジャンプ
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);

        // ★ 重力
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
