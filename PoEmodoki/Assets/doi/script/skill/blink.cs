#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;

public class blink : BaseSkill
{
    float distance;          // ブリンク距離
    float ct;          // クールタイム
    GameObject effect;

    bool isReady = true;

    // ===== 初期化 =====
    public override void Setup(SkillStatus status)
    {
        distance = status.length;   
       ct = status.ct;  
        effect = status.effect;
    }

    public override void EnemySetup(EnemyStatus Estatus) { }

    // ===== Player が使う =====
    public override void UseSkill(PlayerCon con)
    {
        if (!isReady) return;

        Vector3 startPos = con.transform.position;

        // 方向（移動入力があるならその方向、無いなら前方）
        Vector3 dir = GetBlinkDirection(con);
        if (dir.sqrMagnitude < 0.0001f) dir = con.transform.forward;

        Vector3 rawTarget = startPos + dir.normalized * distance;

        // NavMeshがあるなら、NavMesh上の安全な点に補正
        Vector3 targetPos = ResolveBlinkTarget(rawTarget);

        // 実移動
        WarpOrMove(con.gameObject, targetPos);

        // エフェクト
        SpawnEffect(startPos);
        SpawnEffect(targetPos);

        // クールタイム開始
        con.StartCoroutine(CooldownRoutine());
    }

    // ===== Enemy が使う =====
    public override void EnemyUseSkill(Enemy enemy, SkillStatus status)
    {
      
        float d = status.length;

        Vector3 dir = (enemy.Player.transform.position - enemy.transform.position).normalized;
        Vector3 rawTarget = enemy.transform.position + dir * d;

        Vector3 targetPos = ResolveBlinkTarget(rawTarget);

        WarpOrMove(enemy.gameObject, targetPos);
        SpawnEffect(targetPos);
    }

   
    Vector3 GetBlinkDirection(PlayerCon con)
    {
            return con.transform.forward;
    }

   
    Vector3 ResolveBlinkTarget(Vector3 rawTarget)
    {
       
        NavMeshHit hit;
        if (NavMesh.SamplePosition(rawTarget, out hit, 2.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return rawTarget;
    }

   
    void WarpOrMove(GameObject obj, Vector3 targetPos)
    {
        var agent = obj.GetComponent<NavMeshAgent>();
        if (agent != null && agent.enabled)
        {
            agent.Warp(targetPos);
        }
        else
        {
            obj.transform.position = targetPos;
        }
    }

    void SpawnEffect(Vector3 pos)
    {
        if (effect == null) return;
        GameObject.Instantiate(effect, pos, Quaternion.identity);
    }

    System.Collections.IEnumerator CooldownRoutine()
    {
        isReady = false;
        yield return new WaitForSeconds(ct <= 0 ? 0.5f : ct);
        isReady = true;
    }
}