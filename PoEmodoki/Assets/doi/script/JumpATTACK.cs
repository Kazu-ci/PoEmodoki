using System.Collections;
using UnityEngine;

public class JumpATTACK : BaseSkill
{
    [Header("Skill Params")]
    float jumpHeight;
    float airTime;
    float dropSpeed;
    float shockRange;
    float damage;

    GameObject effect;

    bool isJumping = false;

 
    Transform lastAttacker;

 
    public override void Setup(SkillStatus status)
    {
        jumpHeight = status.height;
        airTime = status.airtime;
        dropSpeed = status.speed;
        shockRange = status.lenge;  
        damage = status.atk;
        effect = status.effect;
    }

    public override void EnemySetup(EnemyStatus Estatus)
    {
       
    }

    
    public override void UseSkill(PlayerCon con)
    {
        if (isJumping) return;

        
        Vector3 dropPos = con.transform.position;

        con.StartCoroutine(JumpAttackRoutine(
            attackerTransform: con.transform,
            dropTargetWorldPos: dropPos,
            hitTargetTag: "Enemy"
        ));
    }

    
    public override void EnemyUseSkill(Enemy enemy, SkillStatus status)
    {
        if (isJumping) return;

        Transform playerTf = enemy.Player != null ? enemy.Player.transform : null;
        if (playerTf == null) return;

        Vector3 dropPos = playerTf.position;

        enemy.StartCoroutine(JumpAttackRoutine(
            attackerTransform: enemy.transform,
            dropTargetWorldPos: dropPos,
            hitTargetTag: "Player"     
        ));
    }

   
    IEnumerator JumpAttackRoutine(Transform attackerTransform, Vector3 dropTargetWorldPos, string hitTargetTag)
    {
        isJumping = true;
        lastAttacker = attackerTransform;

        Vector3 startPos = attackerTransform.position;

       
        float t = 0f;
        while (t < airTime)
        {
            float rate = t / airTime;
            float y = Mathf.Lerp(startPos.y, startPos.y + jumpHeight, rate);

            attackerTransform.position = new Vector3(startPos.x, y, startPos.z);

            t += Time.deltaTime;
            yield return null;
        }

        Vector3 dropTarget = new Vector3(dropTargetWorldPos.x, dropTargetWorldPos.y, dropTargetWorldPos.z);

        while (attackerTransform.position.y > dropTarget.y + 0.05f)
        {
            attackerTransform.position = Vector3.MoveTowards(
                attackerTransform.position,
                dropTarget,
                dropSpeed * Time.deltaTime
            );
            yield return null;
        }

      
        attackerTransform.position = new Vector3(dropTarget.x, dropTarget.y, dropTarget.z);

        
        ShockWave(attackerTransform.position, hitTargetTag);

        isJumping = false;
    }

    
    void ShockWave(Vector3 center, string hitTargetTag)
    {
        Collider[] hits = Physics.OverlapSphere(center, shockRange);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag(hitTargetTag)) continue;

        }

        if (effect != null)
        {
            GameObject fx = Instantiate(effect, center, Quaternion.identity);

            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }
    }

   
    void OnDrawGizmosSelected()
    {
        if (lastAttacker == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastAttacker.position, shockRange);
    }
}
