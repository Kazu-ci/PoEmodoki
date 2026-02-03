using UnityEngine;

public class OnHitDamage : MonoBehaviour
{
    [Header("攻撃者")]
    [SerializeField] private Enemy ownerEnemy;
    [SerializeField] private PlayerCon ownerPlayer;

    [Header("ダメージ")]
    [SerializeField] private float damage = 10f;

    private bool hasHit;

    private void OnEnable()
    {
        hasHit = false; // 攻撃開始時にリセット
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // ====================
        // プレイヤーに当たった
        // ====================
        PlayerCon player = other.GetComponent<PlayerCon>();
        if (player != null && ownerEnemy != null)
        {
           
            float finalDamage = ownerEnemy.GetDamage();
            player.TakeDamage(new DamageData(finalDamage));
            Debug.Log("プレイヤーダメージ");
            hasHit = true;
            return;
        }

        // ====================
        // 敵に当たった
        // ====================
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemy != ownerEnemy && ownerPlayer != null)
        {
            float finalDamage = damage;
            enemy.TakeDamage(new DamageData(finalDamage));
            Debug.Log("敵ダメージ");
            hasHit = true;
        }
    }
}