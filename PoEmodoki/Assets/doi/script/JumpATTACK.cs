using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class JumpATTACK : MonoBehaviour
{
    public Transform player;
    public float jumpHeight = 6f;      // どれくらい上に飛ぶか
    public float airTime = 0.7f;       // 滞空時間
    public float dropSpeed = 15f;      // 落下速度
    public float shockRange = 4f;      // 衝撃波の範囲
    public int damage = 20;
    public GameObject effect;
    private bool isJumping = false;
    public GameObject enemy;
    
    public void StartJumpAttack()
    {
        if (!isJumping)
            StartCoroutine(JumpAttackRoutine());
    }

    IEnumerator JumpAttackRoutine()
    {
        isJumping = true;
        Vector3 targetPos = player.position;
        float t = 0;
        Vector3 startPos = enemy.transform.position;

        while (t < airTime)
        {
            float y = Mathf.Lerp(startPos.y, startPos.y + jumpHeight, t / airTime);
            enemy.transform.position = new Vector3(startPos.x, y, startPos.z);
            t += Time.deltaTime;
            yield return null;
        }

        // ▼ 記録したプレイヤー位置へ落下
        while (enemy.transform.position.y > targetPos.y + 0.2f)
        {
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                new Vector3(targetPos.x, targetPos.y, targetPos.z),
                dropSpeed * Time.deltaTime
            );
            yield return null;
        }

        ShockWave();

        isJumping = false;
    }
    void ShockWave()
    {
    
        Collider[] hits = Physics.OverlapSphere(enemy.transform.position, shockRange);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
               //damage
            }
        }
       Instantiate(effect, enemy.transform.position, Quaternion.identity);
        effect.GetComponent<ParticleSystem>().Play();

    }
}
