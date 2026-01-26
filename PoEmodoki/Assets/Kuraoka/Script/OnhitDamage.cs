using UnityEngine;
using Unity.VisualScripting;
public class OnhitDamage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Enemy enemy;
    [SerializeField] private float Damage;
    public void OnTriggerEnter(Collider other)
    {
        //  プレイヤーへのダメージ
        var player=other.GetComponent<PlayerCon>();
        if(player != null )
        {
            float finalDmage=(enemy!=null)?enemy.Getdaamge():Damage;
            DamageData damageData= new DamageData(finalDmage);
            player.TakeDamage(damageData);
        }

        //敵へのダメージ
        var otherEnemy=other.GetComponent<Enemy>();
        if(otherEnemy != null &&otherEnemy!=enemy)
        {
            float finalDamage = (enemy != null) ? enemy.Getdaamge() : Damage;
            DamageData damageData= new DamageData(finalDamage);
            otherEnemy.TakeDamage(damageData);
        }
    }
}
