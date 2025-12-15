using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSkill : IUseSkill
{
    abstract public void Setup(SkillStatus status);
    abstract public void UseSkill(PlayerCon con);
}

public interface IUseSkill 
{
    void Setup(SkillStatus status);
    //‚±‚¢‚Â‚Í•Ö—˜‚Å‚·B
    void UseSkill(PlayerCon con);
}
