using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSkill : MonoBehaviour, IUseSkill
{
    private SerializedObject sSkill;
    public Action<GameObject> skillAction => UseSkill;

    abstract protected void UseSkill(GameObject obj);
}

public interface IUseSkill 
{
    //‚±‚¢‚Â‚Í•Ö—˜‚Å‚·B
    public Action<GameObject> skillAction { get; }
}
