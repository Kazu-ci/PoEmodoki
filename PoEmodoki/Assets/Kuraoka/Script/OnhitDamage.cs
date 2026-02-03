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

    }
}