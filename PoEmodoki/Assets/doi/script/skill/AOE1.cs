#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class AOE1 : BaseSkill
{
    GameObject effect;
    public float Distance;
    Vector3 forwardDirection;
    Vector3 offset;
    Vector3 point;

    public override void Setup(SkillStatus status)
    {
        effect = status.effect;
    }
    public override void EnemySetup(EnemyStatus Estatus)
    {
        
    }

    public override void UseSkill(PlayerCon con)
    {
        forwardDirection = con.transform.forward;
        offset = forwardDirection * Distance;
        point = con.transform.position /*+ offset*/;
        point.y = 0;
        con.InstanciateSkillEffect(effect, point, Quaternion.Euler(-90, 0, 0));
        //サウンド
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(SoundManager.SE.AOE);
        }
    }


    //敵用
    public override void EnemyUseSkill(Enemy enemy, SkillStatus status)
    {
        Vector3 Enemypoint = enemy.transform.position;
        enemy.InstanciateSkillEffect(effect, Enemypoint, Quaternion.Euler(-90, 0, 0));

        float radius = status.length;

        Collider[] hits =
            Physics.OverlapSphere(enemy.transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<PlayerCon>();
                // player?.TakeDamage((int)status.atk);
            }
        }

    }
    
}
 