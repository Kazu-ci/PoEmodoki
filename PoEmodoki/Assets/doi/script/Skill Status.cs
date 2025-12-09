using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName =("skills"))]
public class SkillStatus : ScriptableObject
{
    public float atk;
    public float def;
    public float time;
    public float ct;//cooltime
    public float speed;
    public float lenge;
    public float length;
    public Image Icon;
    public string name;
    public Sprite sprite;
    public GameObject skillPre;
}
