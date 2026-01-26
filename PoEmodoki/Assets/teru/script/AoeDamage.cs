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
        Vector3 playerPos = playerAnchor.Value.position;
        Vector3 myPos = transform.position;
        playerPos.y = myPos.y;

        offset = myPos - playerPos;
        sqrLen = offset.sqrMagnitude;
        // 判定用の閾値
        float innerCheck = (Ir - Pr) * (Ir - Pr);
        float outerCheck = (Or + Pr) * (Or + Pr);
        //Debug.Log($"現在の2乗距離: {sqrLen} | 判定範囲: {innerCheck} ～ {outerCheck}");
        bool isInside = (sqrLen > innerCheck && sqrLen < outerCheck);

        if (isInside)
        {
            Debug.Log("oa");
            if (!hit)
            {
                ApplyDamage(); 
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
