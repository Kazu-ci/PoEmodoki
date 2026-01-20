using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum SKILL
{
    None = -1,
    AOE,
    Tossin,
    Bomb,
    Blink,
}

[CreateAssetMenu(menuName =("skills"))]
public class SkillStatus : ScriptableObject
{
    public BaseSkill skill;//“G—p
    public float atk;
    public SKILL skillId;
    public float def;
    public float time;
    public float ct;//cooltime
    public float speed;
    public float lenge;
    public float length;
    public Image Icon;
    public string name;
    public string Skillname;
    public Sprite sprite;
    public GameObject effect;
}
