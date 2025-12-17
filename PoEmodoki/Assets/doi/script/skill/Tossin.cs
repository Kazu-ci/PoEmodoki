using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;

public class Tossin : BaseSkill
{
    float speed;
    public KeyCode SpawnKey = KeyCode.F;
    public float dashDuration = 0.2f;
    float dashTimer;
    float Ct;
    float Count;
    bool ISDASH = false;
    float h, v;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Setup(SkillStatus status)
    {
        speed = status.speed;
        Ct = status.ct;
    }

    public override void UseSkill(PlayerCon con)
    {
            isDash();
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        if (ISDASH&&Count>Ct)
        {
            dashTimer -= Time.deltaTime;
            Vector3 inputDirection = new Vector3(h, 0, v).normalized;
            Quaternion characterRotation = con.transform.rotation;
            Vector3 worldMoveDirection = characterRotation * inputDirection;
            Vector3 moveVector = worldMoveDirection * speed;
            // TODO: 移動させる方法を考えて.
            //CC.Move(moveVector * Time.deltaTime);
            if (dashTimer < 0)
            {
                ISDASH = false;
            }
            Count = 0;
        }
        if(Count<=Ct)
        {
            ++Count;
        }
    }
    void isDash()
    {
        dashTimer = dashDuration;
        ISDASH = true;
    }

    public override void EnemyUseSkill(Enemy enemy, SkillStatus status)
    {
        NavMeshAgent agent = enemy.Agent;
        GameObject player = enemy.Player;
        // ステータス反映
        speed = status.speed;
        dashDuration = status.time;

        // プレイヤー方向へダッシュ
        Vector3 dir = (enemy.Player.transform.position - enemy.transform.position).normalized;
        dir.y = 0;

        enemy.StartCoroutine(Dash(enemy, dir));
    }
    IEnumerator Dash(Enemy enemy, Vector3 dir)
    {
        float timer = 0f;

        while (timer < dashDuration)
        {
            enemy.transform.position += dir * speed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
    }
  }

