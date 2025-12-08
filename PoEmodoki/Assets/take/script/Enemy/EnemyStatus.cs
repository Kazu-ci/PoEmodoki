using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyStatus", menuName = "Enemy/Status")]
public class EnemyStatus : ScriptableObject
{
    [Header("“G‚ÌƒXƒe[ƒ^ƒX")]
    public int EnemyHp;                 //“G‚ÌHP
    public int EnemyAtk;                //“G‚ÌUŒ‚—Í
    public int EnemyDefense;            //“G‚Ì–hŒä—Í
    public float EnemySpeed;            //“G‚ÌˆÚ“®‘¬“x
    public float EnemyAtkSpeed;         //“G‚ÌUŒ‚‘¬“x
    public float EnemyCastSpeed;        //“G‚Ì‰r¥‘¬“x
    public float EnemySens;
    public float EnemyLength;           //“G‚ÌË’ö
    public float EnemyElementDefense;   //“G‚Ì‘®«‘Ï«
    public float EnemyFov;              //“G‚Ì‹–ìŠp
    public string EnemyName;            //“G‚Ì–¼‘O
}
