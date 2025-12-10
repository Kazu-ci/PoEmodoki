#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class AOE : BaseSkill
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

    public override void UseSkill(PlayerCon con)
    {
        forwardDirection = con.transform.forward;
        offset = forwardDirection * Distance;
        point = con.transform.position /*+ offset*/;
        point.y = 0;
        con.InstanciateSkillEffect(effect, point, Quaternion.Euler(-90, 0, 0));
    }
}
 