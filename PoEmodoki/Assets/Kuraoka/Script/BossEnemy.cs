using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.UI.GridLayoutGroup;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class BossEnemy : Enemy
{
    StateMachine<BossEnemy> stateMachine;
    [SerializeField] EnemyStatus BossStatus;
    [SerializeField] GameObject[] mobEnemy;
    [SerializeField] private List<string> attackStates;
    [SerializeField] private List<GameObject> effects;
    [SerializeField] private List<Collider> attackColliders;
    [SerializeField] float rushSpeed;
    [SerializeField] float stiffnessTime;
    private Dictionary<string, Collider> colliderDict;
    private Dictionary<string, GameObject> effectDict;
    [SerializeField] private List<SkillStatus> skills;
    private SkillStatus currentSkill;
#if UNITY_EDITOR
    private SerializedObject seliarizeBossStatus;
#endif

    private enum BossSkillType
    {
        CloseAoE,       // 近距離範囲攻撃
        AreaWide,       // エリア全体攻撃
        Projectile      // 遠距離魔法
    }

    private enum EnemyState
    {
        Idle,
        Chase,
        Vigilance,
        Attack,
        Rotate,
        Sumon,
        Skill,
        Stiffness,
        Hit,
        Dead
    }
    void Awake()
    {
     /*   effectDict = new Dictionary<string, GameObject>();
        colliderDict = new Dictionary<string, Collider>();
        for (int i = 0; i < Mathf.Min(attackStates.Count, effects.Count); i++)
        {
            effectDict[attackStates[i]] = effects[i];
            effects[i].SetActive(false);
            if (i < attackColliders.Count && attackColliders[i] != null)
                colliderDict[attackStates[i]] = attackColliders[i];
        }

        foreach (var c in attackColliders) c.enabled = false;*/
    }
    void Start()
    {
        MaxHP = BossStatus.EnemyHp;
        currentHP = MaxHP;
        AttackRange = BossStatus.EnemyLength;
        AttackSpeed = BossStatus.EnemyAtkSpeed;
        Strength = BossStatus.EnemyAtk;
        MoveSpeed = BossStatus.EnemySpeed;
 
        navMeshAgent = GetComponent<NavMeshAgent>();
       
        stateMachine = new StateMachine<BossEnemy>(this);
        stateMachine.Add<IdleState>((int)EnemyState.Idle);
        stateMachine.Add<ChaseState>((int)EnemyState.Chase);
        stateMachine.Add<VigilanceState>((int)EnemyState.Vigilance);
        stateMachine.Add<AttackState>((int)EnemyState.Attack);
        stateMachine.Add<RotateState>((int)EnemyState.Rotate);
        stateMachine.Add<SumonState>((int)EnemyState.Sumon);
        stateMachine.Add<SkillState>((int)EnemyState.Skill);
        stateMachine.Add<StiffnessState>((int)EnemyState.Stiffness);
        stateMachine.Add<HitState>((int)EnemyState.Hit);
        stateMachine.Add<DeadState>((int)EnemyState.Dead);
        stateMachine.Onstart((int)EnemyState.Idle);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        OnEffects();
        if(currentHP <= 0) { stateMachine.ChangeState((int)EnemyState.Dead); }
        stateMachine.OnUpdate();
    }
    public override void OnAttackSet()
    {
        attackColliders.ForEach(c => c.enabled = false);

        /*var state = animator.GetCurrentAnimatorStateInfo(0);
        foreach (var kv in colliderDict)
        {
            if (state.IsName(kv.Key))
            {
                kv.Value.enabled = true;
                break;
            }
        }*/
    }

    public override void OnAttackEnd() => attackColliders.ForEach(c => c.enabled = false);
    private void OnEffects()
    {/*
        var state = animator.GetCurrentAnimatorStateInfo(0);
        foreach (var kvp in effectDict)
            kvp.Value.SetActive(state.IsName(kvp.Key) && state.normalizedTime < 1f);*/
    }
    public override void OnSumon()
    {
        for (int i = 0; i < mobEnemy.Length; i++)
        {
            if (mobEnemy[i] == null) continue; // nullチェック
            float angle = Random.Range(-90, 90);
            Quaternion rot = Quaternion.Euler(0, angle, 0);
            Vector3 dir = rot * transform.forward;
            Vector3 spawnPos = transform.position + dir.normalized * 5;
            Instantiate(mobEnemy[i], spawnPos, Quaternion.identity);
        }
    }
    private BossSkillType GetSkillType(SkillStatus skill)
    {
        string n = skill.name.ToLower();

        if (n.Contains("aoe")) return BossSkillType.CloseAoE;
        if (n.Contains("area")) return BossSkillType.AreaWide;
        if (n.Contains("magic") || n.Contains("projectile")) return BossSkillType.Projectile;

        return BossSkillType.Projectile;
    }
    public void BossSkill_CloseAoE(SkillStatus skill)
    {
        float radius = skill.length;

        Collider[] hits = Physics.OverlapSphere(this.transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<PlayerCon>();
                //player?.TakeDamage((int)skill.atk);
            }
        }

        if (skill.skillPre != null)
            Instantiate(skill.skillPre, transform.position, Quaternion.identity);
    }

    public void BossSkill_AreaWide(SkillStatus skill)
    {
        float radius = skill.length * 3f;

        Collider[] hits = Physics.OverlapSphere(this.transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<PlayerCon>();
                //player?.TakeDamage((int)skill.atk);
            }
        }

        if (skill.skillPre != null)
            Instantiate(skill.skillPre, transform.position, Quaternion.identity);
    }


    public void BossSkill_Projectile(SkillStatus skill)
    {
        if (skill.skillPre == null) return;

        Vector3 spawnPos =
            transform.position +
            transform.forward * 1.5f +
            Vector3.up * 1.2f;

        GameObject proj = Instantiate(skill.skillPre, spawnPos, transform.rotation);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = dir * skill.speed;
        }

        // 消滅時間（射程 / 弾速）
        if (skill.speed > 0 && skill.length > 0)
            Destroy(proj, skill.length / skill.speed);
    }
    private class IdleState : StateMachine<BossEnemy>.StateBase
    {
        float cDis;
        public override void OnStart()
        {
            //Owner.animator.SetTrigger("Idle");
            cDis = Owner.lookPlayerDir;
        }
        public override void OnUpdate()
        {
            StateMachine.ChangeState((int)EnemyState.Chase);
        }
        public override void OnEnd()
        {
            //Owner.animator.ResetTrigger("Idle");
        }
    }

    private class ChaseState : StateMachine<BossEnemy>.StateBase
    {
        NavMeshAgent navMeshAgent;
        public override void OnStart()
        {
           //Owner.animator.SetTrigger("Idle");
            navMeshAgent = Owner.navMeshAgent;
            navMeshAgent.isStopped = false;
        }
        public override void OnUpdate()
        {
            Vector3 playerPos = Owner.player.transform.position;
            navMeshAgent.SetDestination(playerPos);
            if (Owner.Getdistance() <= Owner.AttackRange)
            {
                navMeshAgent.isStopped = true;
                if (Probability(70)) { StateMachine.ChangeState((int)EnemyState.Attack); }
                if (Probability(30)) { StateMachine.ChangeState((int)EnemyState.Rotate); }
            }
        }
        public override void OnEnd()
        {
            //Owner.animator.ResetTrigger("Idle");
        }
    }
    private class VigilanceState : StateMachine<BossEnemy>.StateBase
    {
        float time;
        float mTime;

        public float roamRadius = 5f;      // プレイヤーを中心とした円の半径
        public float roamChangeInterval = 2f;// ランダム位置を更新する間隔

        private Vector3 roamTarget;      // 今の円内ターゲット位置
        private float roamTimer;
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = false;
           // Owner.animator.SetTrigger("Idle");
            time = 0;
            mTime = Random.Range(4, 6);
            PickNewRoamPosition();
        }
        public override void OnUpdate()
        {
            if (time > mTime)
            {
                time = 0;
                if (Probability(60)) { StateMachine.ChangeState((int)EnemyState.Sumon); }
                if (Probability(40)) { StateMachine.ChangeState((int)EnemyState.Rotate); }
            }
            time += Time.deltaTime;

            float distance = Owner.Getdistance();

            if (distance < Owner.AttackRange)
            {
                Vector3 dir = (Owner.transform.position - Owner.player.transform.position).normalized;
                Vector3 retreatPos = Owner.player.transform.position + dir * Owner.AttackRange * 2;
                Owner.navMeshAgent.SetDestination(retreatPos);
            }
            else
            {
                // ======================== 
                // 円内をランダムに回る 
                // ========================
                roamTimer -= Time.deltaTime;
                if (roamTimer <= 0f)
                {
                    PickNewRoamPosition();
                }
                Owner.navMeshAgent.SetDestination(roamTarget);
            }

            Vector3 lookDir = Owner.player.transform.position - Owner.transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, Quaternion.LookRotation(lookDir), 0.1f);
            }
        }
        public override void OnEnd()
        {
           // Owner.animator.ResetTrigger("Idle");
        }
        void PickNewRoamPosition()
        {
            roamTimer = roamChangeInterval;
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float r = Random.Range(0f, roamRadius);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * r, 0, Mathf.Sin(angle) * r);
            roamTarget = Owner.player.transform.position + offset;
        }
    }

    private class AttackState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.transform.LookAt(Owner.player.transform.position);
           // Owner.animator.SetTrigger("Attack");
            Owner.navMeshAgent.isStopped = true;
        }
        public override void OnUpdate()
        {
            if (Owner.AnimationEnd("Attack"))
            {
                if (Owner.Getdistance() <= Owner.AttackRange)
                {
                    if (Probability(30)) { StateMachine.ChangeState((int)EnemyState.Attack); }
                    if (Probability(50)) { StateMachine.ChangeState((int)EnemyState.Rotate); }
                    if (Probability(20)) { StateMachine.ChangeState((int)EnemyState.Vigilance); }
                }
                else
                {
                    if (Probability(20)) { StateMachine.ChangeState((int)EnemyState.Skill); }
                    if (Probability(30)) { StateMachine.ChangeState((int)EnemyState.Vigilance); }
                    if (Probability(50)) { StateMachine.ChangeState((int)EnemyState.Chase); }
                }
            }
        }
        public override void OnEnd()
        {
            //Owner.animator.ResetTrigger("Attack");
        }
    }
    private class RotateState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;
            //Owner.animator.SetTrigger("Attack");
        }
        public override void OnUpdate()
        {
            if ((Owner.AnimationEnd("Attack")))
            {
                if (Owner.Getdistance() <= Owner.AttackRange)
                {
                    if (Probability(30)) { StateMachine.ChangeState((int)EnemyState.Attack); }
                    if (Probability(20)) { StateMachine.ChangeState((int)EnemyState.Skill); }
                    if (Probability(50)) { StateMachine.ChangeState((int)EnemyState.Vigilance); }
                }
                else
                {
                    if (Probability(50)) { StateMachine.ChangeState((int)EnemyState.Skill); }
                    if (Probability(50)) { StateMachine.ChangeState((int)EnemyState.Chase); }
                }
            }
        }
        public override void OnEnd()
        {
           // Owner.animator.ResetTrigger("Attack");
        }
    }
    private class SumonState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;
           // Owner.animator.SetTrigger("Sumon");

        }
        public override void OnUpdate()
        {
            if (Owner.AnimationEnd("Sumon"))
            {
                if (Probability(20)) { StateMachine.ChangeState((int)EnemyState.Vigilance); }
                if (Probability(80))
                {
                    if (Owner.Getdistance() <= Owner.AttackRange)
                    {
                        if (Probability(40)) { StateMachine.ChangeState((int)EnemyState.Rotate); }
                        if (Probability(60)) { StateMachine.ChangeState((int)EnemyState.Attack); }
                    }
                    else
                    {
                        StateMachine.ChangeState((int)EnemyState.Skill);
                    }
                }
            }
        }
        public override void OnEnd()
        {
            //Owner.animator.ResetTrigger("Sumon");
        }
    }
    private class SkillState : StateMachine<BossEnemy>.StateBase
    {
        private SkillStatus skill;
        private BossSkillType skillType;
        private float timer;
        private float originalAtk;

        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;

            // スキルをランダム選択
            skill = Owner.skills[Random.Range(0, Owner.skills.Count)];
            Owner.currentSkill = skill;

            // ボス専用スキルタイプ判定
            skillType = Owner.GetSkillType(skill);

            // 攻撃力をスキル用に差し替え
            originalAtk = Owner.Strength;
            Owner.Strength = (int)skill.atk;

            // プレイヤーの方向へ向く
            Vector3 lookPos = Owner.player.transform.position;
            lookPos.y = Owner.transform.position.y;
            Owner.transform.LookAt(lookPos);

            Owner.animator.SetTrigger("Skill");

            // スキル使用
            switch (skillType)
            {
                case BossSkillType.CloseAoE:
                    Owner.BossSkill_CloseAoE(skill);
                    break;

                case BossSkillType.AreaWide:
                    Owner.BossSkill_AreaWide(skill);
                    break;

                case BossSkillType.Projectile:
                    Owner.BossSkill_Projectile(skill);
                    break;
            }

            timer = 0f;
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            // アニメーション終了 または スキル硬直終了
            if (Owner.AnimationEnd("Skill") && timer >= skill.time)
            {
                if (Owner.Getdistance() <= Owner.AttackRange)
                    StateMachine.ChangeState((int)EnemyState.Vigilance);
                else
                    StateMachine.ChangeState((int)EnemyState.Chase);
            }
        }

        public override void OnEnd()
        {
            Owner.animator.ResetTrigger("Skill");

            // 攻撃力を戻す
            Owner.Strength = originalAtk;
        }
    }

    private class StiffnessState : StateMachine<BossEnemy>.StateBase
    {
        float time;
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;
            //Owner.animator.SetTrigger("Idle");
            time = 0;
        }
        public override void OnUpdate()
        {
            if (time >= Owner.stiffnessTime)
            {
                if (Probability(50)) { StateMachine.ChangeState((int)EnemyState.Vigilance); }
                if (Probability(50)) { StateMachine.ChangeState((int)EnemyState.Chase); }
                time = 0;
            }
            time += Time.deltaTime;
        }
        public override void OnEnd()
        {
           // Owner.animator.ResetTrigger("Idle");
        }

    }

    private class HitState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            //Owner.animator.SetTrigger("Damage");
        }
        public override void OnUpdate()
        {
            if (Owner.AnimationEnd("")) { StateMachine.ChangeState((int)EnemyState.Idle); }
        }
        public override void OnEnd()
        {
           // Owner.animator.ResetTrigger("Damage");
        }
    }
    private class DeadState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
          //  Owner.animator.SetTrigger("Dead");
        }
        public override void OnUpdate()
        {
            if (Owner.AnimationEnd("Dead"))
            {
                Owner.OnDead();
            }
        }
        public override void OnEnd()
        {
            //Owner.animator.ResetTrigger("Dead");
        }
    }

    public override int TakeDamage(DamageData dmg)
    {
        int damageTaken = base.TakeDamage(dmg);

        if (currentHP <= 0)
        {
            
        }
        return damageTaken;
    }


#if UNITY_EDITOR

public void DrawRunningStatusGUI()
    {
        EditorGUILayout.FloatField("現在のHP:", currentHP);
        EditorGUILayout.FloatField("HPの最大値:", MaxHP);
        EditorGUILayout.FloatField("移動速度:", MoveSpeed);
        EditorGUILayout.FloatField("攻撃力:", Strength);
    }
#endif
#if UNITY_EDITOR
    public SerializedObject GetSerializedBaseStatus()
    {
        if(BossStatus == null)
        {
            return null;
        }

        if(seliarizeBossStatus == null || seliarizeBossStatus.targetObject != BossStatus)
        {
            seliarizeBossStatus = new SerializedObject(BossStatus);
        }
        return seliarizeBossStatus;
    }
#endif
}
