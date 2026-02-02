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

    // デバッグ描画用（最後にスキルを使った側）
    Transform lastAttacker;

    // ===== 初期化 =====
    public override void Setup(SkillStatus status)
    {
        // ここは SkillStatus 側の変数名に合わせてね
        jumpHeight = status.height;
        airTime = status.airTime;
        dropSpeed = status.speed;
        shockRange = status.lenge;   // ← spellミスっぽいけど、元に合わせてる
        damage = status.atk;
        effect = status.effect;
    }

    public override void EnemySetup(EnemyStatus Estatus)
    {
        // 必要ならここでEnemy専用の補正を入れる
    }

    // ===== Player がスキルを使う =====
    public override void UseSkill(PlayerCon con)
    {
        if (isJumping) return;

        // 自分の位置に落ちるなら target = 自分
        // 前方に落としたいなら con.transform.position + con.transform.forward * 落下距離 にする
        Vector3 dropPos = con.transform.position;

        con.StartCoroutine(JumpAttackRoutine(
            attackerTransform: con.transform,
            dropTargetWorldPos: dropPos,
            hitTargetTag: "Enemy"      // 敵に当てる
        ));
    }

    // ===== Enemy がスキルを使う =====
    public override void EnemyUseSkill(Enemy enemy, SkillStatus status)
    {
        if (isJumping) return;

        Transform playerTf = enemy.Player != null ? enemy.Player.transform : null;
        if (playerTf == null) return;

        Vector3 dropPos = playerTf.position;

        enemy.StartCoroutine(JumpAttackRoutine(
            attackerTransform: enemy.transform,
            dropTargetWorldPos: dropPos,
            hitTargetTag: "Player"     // プレイヤーに当てる
        ));
    }

    // ===== ジャンプ攻撃本体（共通） =====
    IEnumerator JumpAttackRoutine(Transform attackerTransform, Vector3 dropTargetWorldPos, string hitTargetTag)
    {
        isJumping = true;
        lastAttacker = attackerTransform;

        Vector3 startPos = attackerTransform.position;

        // ▲ 上昇：その場で真上にジャンプ
        float t = 0f;
        while (t < airTime)
        {
            float rate = t / airTime;
            float y = Mathf.Lerp(startPos.y, startPos.y + jumpHeight, rate);

            attackerTransform.position = new Vector3(startPos.x, y, startPos.z);

            t += Time.deltaTime;
            yield return null;
        }

        // ▼ 落下：指定した地点へ落ちる（XY合わせ）
        // 地面の高さに合わせたいなら dropTargetWorldPos.y を地面Yにしてね
        Vector3 dropTarget = new Vector3(dropTargetWorldPos.x, dropTargetWorldPos.y, dropTargetWorldPos.z);

        // ちょい上まで落とす（同じ高さだとMoveTowardsが止まらないケース対策）
        while (attackerTransform.position.y > dropTarget.y + 0.05f)
        {
            attackerTransform.position = Vector3.MoveTowards(
                attackerTransform.position,
                dropTarget,
                dropSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 最終的に着地位置を固定
        attackerTransform.position = new Vector3(dropTarget.x, dropTarget.y, dropTarget.z);

        // 💥 衝撃波
        ShockWave(attackerTransform.position, hitTargetTag);

        isJumping = false;
    }

    // ===== 衝撃波処理 =====
    void ShockWave(Vector3 center, string hitTargetTag)
    {
        Collider[] hits = Physics.OverlapSphere(center, shockRange);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag(hitTargetTag)) continue;

            // ここでダメージ処理
            // PlayerCon / Enemy に合わせて書き換えてね
            // 例:
            // if (hitTargetTag == "Player")
            //     hit.GetComponent<PlayerCon>()?.TakeDamage(damage);
            // else
            //     hit.GetComponent<Enemy>()?.TakeDamage(damage);
        }

        if (effect != null)
        {
            GameObject fx = UnityEngine.GameObject.Instantiate(effect, center, Quaternion.identity);

            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }
    }

    // ===== デバッグ用 =====
    void OnDrawGizmosSelected()
    {
        if (lastAttacker == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastAttacker.position, shockRange);
    }
}
