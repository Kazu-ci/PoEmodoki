#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class blink : BaseSkill
{
    [SerializeField] SkillStatus skill;
    float speed;
    bool On=true;
    bool used = false;
   void Start()
    {
        speed = 5;
    }
    void Blink(PlayerCon con)
    {
        con.Getvalue(speed);
        On = false;
    }
    public override void Setup(SkillStatus status)
    {
    }
    public override void EnemySetup(EnemyStatus Estatus)
    {
    }
    public override void UseSkill(PlayerCon con)
    {
        if ( On == true)
        {
            Blink(con);
            used = true;
        }
        else
        {
            used = false;
        }
        /*if (used == true)
        {
            ct = data.ct;
        }
        else
        {
            --ct;
        }*/
    }

    public override void EnemyUseSkill(Enemy enemy, SkillStatus status)
    {
        NavMeshAgent agent = enemy.Agent;
        GameObject player = enemy.Player;
        // ブリンク距離
        float distance = status.length;   // SkillStatus に length がある前提

        // プレイヤー方向へ
        Vector3 dir = (enemy.Player.transform.position - enemy.transform.position).normalized;

        // 移動先
        Vector3 targetPos = enemy.transform.position + dir * distance;

        // NavMeshAgent を使っているなら一旦止める
        if (enemy.Agent != null)
        {
            enemy.Agent.Warp(targetPos);
        }
        else
        {
            enemy.transform.position = targetPos;
        }

        // エフェクト
        if (status.effect != null)
        {
            GameObject.Instantiate(
                status.effect,
                enemy.transform.position,
                Quaternion.identity
            );
        }
    }
}
