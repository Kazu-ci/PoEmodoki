using UnityEngine;

public class AoeDamage : MonoBehaviour
{
    [SerializeField] protected PlayerAnchor playerAnchor;
    [SerializeField] private float Damage;
    private float Ir = 1.5f;
    private float Or = 2.5f;
    private float Pr = 0.5f;
    private Vector3 offset;
    float sqrLen;
    bool hit = false;

    // Update is called once per frame
    void Update()
    {
        offset = transform.position - playerAnchor.Value.position;
        sqrLen = offset.sqrMagnitude;

        // 判定用の閾値（前回の回答の通り、ドーナツ状の範囲判定）
        float innerCheck = (Ir - Pr) * (Ir - Pr);
        float outerCheck = (Or + Pr) * (Or + Pr);

        bool isInside = (sqrLen > innerCheck && sqrLen < outerCheck);

        if (isInside)
        {
            if (!hit)
            {
                ApplyDamage(); // ダメージ処理を実行
                hit = true;    
            }
        }
        else
        {
            hit = false;
        }
    }
    void ApplyDamage()
    {
        var player = playerAnchor.Value.GetComponent<PlayerCon>();
        if (player != null)
        {
            DamageData damageData = new DamageData(Damage);
            player.TakeDamage(damageData);
            Debug.Log("ダメージを与えました");
        }
    }
}
