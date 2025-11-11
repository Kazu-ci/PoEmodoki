using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyStatus", menuName = "Enemy/Status")]
public class EnemyStatus : ScriptableObject
{
    public int EnemyHp;                 //“G‚ÌHP
    public int EnemyAtk;                //“G‚ÌUŒ‚—Í
    public int EnemyDefense;            //“G‚Ì–hŒä—Í
    public float EnemySpeed;            //“G‚ÌˆÚ“®‘¬“x
    public float EnemyAtkSpeed;         //“G‚ÌUŒ‚‘¬“x
    public float EnemyCastSpeed;        //“G‚Ì‰r¥‘¬“x
    public float EnemyLength;           //“G‚ÌË’ö
    public float EnemyElementDefense;   //“G‚Ì‘®«‘Ï«
}
