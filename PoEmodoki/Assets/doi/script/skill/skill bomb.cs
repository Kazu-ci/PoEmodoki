using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
public class skillbomb : BaseSkill
{
    GameObject effect;
    float initCt;
    float ct;

    public override void Setup(SkillStatus status)
    {
        effect = status.effect;
        initCt = status.ct;
    }
    public override void EnemySetup(EnemyStatus Estatus)
    {
        
    }

    public override void UseSkill(PlayerCon con)
    {
        Vector3 spawnPos = con.transform.position + con.transform.forward * 1.5f;
        spawnPos.y -= -0.48f;
        if (ct < 0)
        {
            con.InstanciateSkillEffect(effect, spawnPos, con.transform.rotation * Quaternion.Euler(-90, 0, 0));
            ct = initCt;
        }
        if (ct >= 0)
        {
            --ct;
        }
    }

    public override void EnemyUseSkill(Enemy enemy, SkillStatus status)
    {
        throw new System.NotImplementedException();
    }
}
