using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class BaseSkill : IUseSkill
{
    abstract public void Setup(SkillStatus status);
    abstract public void UseSkill(PlayerCon con);

    //“G—p
   public abstract  void EnemyUseSkill(Enemy enemy, SkillStatus status);

}

public interface IUseSkill 
{
    void Setup(SkillStatus status);
    //‚±‚¢‚Â‚Í•Ö—˜‚Å‚·B
    void UseSkill(PlayerCon con);


}
