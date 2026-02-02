using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
public class Tossin : BaseSkill
{
    float speed;
    float atk;

    float ct;
    float ctTimer;

    public float dashDuration = 0.2f;
    float dashTimer;
    float hp;
    bool isDashing = false;
    PlayerCon currentCon;
    Vector3 dashDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Setup(SkillStatus status)
    {
        atk = status.atk;
        speed = status.speed;
        ct = status.ct;
    }
    public override void EnemySetup(EnemyStatus Estatus)
    {
        hp = Estatus.EnemyHp;
    }
    public override void EnemyUseSkill(Enemy enemy, SkillStatus status)
    {

    }
    public override void UseSkill(PlayerCon con)
    {
        currentCon = con;
        if (!isDashing && ctTimer <= 0f)
        {
            StartDash(con);
        }
    }
    void StartDash(PlayerCon con)
    {
        isDashing = true;
        dashTimer = dashDuration;
        ctTimer = ct;

        // 入力方向 or 前方向
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v);

        if (inputDir.magnitude > 0.1f)
        {
            dashDirection = con.transform.rotation * inputDir.normalized;
        }
        else
        {
            dashDirection = con.transform.forward;
        }
    }
    void Update()
    {
        if (ctTimer > 0f)
            ctTimer -= Time.deltaTime;
    }
    void LateUpdate()
    {
        if (!isDashing || currentCon == null) return;

        CharacterController cc =
            currentCon.GetComponent<CharacterController>();

        cc.Move(dashDirection * speed * Time.deltaTime);

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("敵に衝突！");
            // ダメージ処理 here
        }
    }
}

