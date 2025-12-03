using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSkill : MonoBehaviour, IUseSkill
{
#if UNITY_EDITOR
    private SerializedObject sSkill;
#endif
    public Action<GameObject> skillAction => UseSkill;

    abstract protected void UseSkill(GameObject obj);
}

public interface IUseSkill 
{
    //Ç±Ç¢Ç¬ÇÕï÷óòÇ≈Ç∑ÅB
    public Action<GameObject> skillAction { get; }
}
