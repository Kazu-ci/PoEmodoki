using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
public class skillbomb : BaseSkill
{
    GameObject effect;
    GameObject bomb;
    float initCt;
    float ct;
    [SerializeField] SkillStatus data;

    public override void Setup(SkillStatus status)
    {
        effect = status.effect;
        initCt = status.ct;
        bomb = data.obj;
    }
    public override void EnemySetup(EnemyStatus Estatus)
    {
        
    }

    public override void UseSkill(PlayerCon con)
    {
        
        Vector3 spawnPos = con.transform.position + con.transform.forward * 1.5f;
        UnityEngine.Object.Instantiate(bomb,spawnPos, Quaternion.Euler(-90, 0, 0));
        Debug.Log("èoåªÇµÇ‹ÇµÇΩ");
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
    void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Object.Destroy(bomb);
    }
    public override void EnemyUseSkill(Enemy enemy, SkillStatus status)
    {
        Vector3 spawnPos = enemy.transform.position + enemy.transform.forward * 1.5f;
        spawnPos.y -= -0.48f;
        if (ct < 0)
        {
            enemy.InstanciateSkillEffect(effect, spawnPos, enemy.transform.rotation * Quaternion.Euler(-90, 0, 0));
            ct = initCt;
        }
        if (ct >= 0)
        {
            --ct;
        }
    }
}
