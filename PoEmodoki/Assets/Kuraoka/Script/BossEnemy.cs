using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.UI.GridLayoutGroup;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.InputSystem.Interactions;

public class BossEnemy : Enemy, IStatusView
{
    [SerializeField] EnemyStatus BossStatus;
    private SerializedObject seliarizeBossStatus;

    StateMachine<BossEnemy> stateMachine;

    [SerializeField] GameObject[] mobEnemy;
    [SerializeField] SkillStatus skills;

    [SerializeField] private List<string> attackStates;
    protected NavMeshAgent navMeshAgent;

    [SerializeField] private List<GameObject> effects;
    [SerializeField] private List<Collider> attackColliders;
    private Dictionary<string, Collider> colliderDict;
    private Dictionary<string, GameObject> effectDict;

    //============================
    // ▼追加：スキル管理データ
    //============================
    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public SkillStatus status; // ScriptableObject
        public float cooldown;
        [HideInInspector] public float lastUsedTime = -999f;
    }

    [SerializeField] private List<SkillData> skillList;
    [SerializeField] private float attackInterval = 2f; // 通常攻撃間隔
    private float lastAttackTime;


    private enum EnemyState
    {
        Idle,
        Chase,
        Vigilance,
        Attack,
        Hit,
        Skill,
        Sumon,
        Dead,
        Rotate,
    }


    private void Awake()
    {
        effectDict = new Dictionary<string, GameObject>();
        colliderDict = new Dictionary<string, Collider>();

        for (int i = 0; i < Mathf.Min(attackStates.Count, effects.Count); i++)
        {
            effectDict[attackStates[i]] = effects[i];
            effects[i].SetActive(false);
        }

        for (int i = 0; i < Mathf.Min(attackStates.Count, attackColliders.Count); i++)
        {
            colliderDict[attackStates[i]] = attackColliders[i];
        }

        attackColliders.ForEach(c => c.enabled = false);
    }


    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        MaxHP = BossStatus.EnemyHp;
        currentHP = MaxHP;
        Strength = BossStatus.EnemyAtk;
        AttackSpeed = BossStatus.EnemyAtkSpeed;
        AttackRange = BossStatus.EnemyLength;
        MoveSpeed = BossStatus.EnemySpeed;
        fov = BossStatus.EnemyFov;
        name = BossStatus.EnemyName;

        stateMachine = new StateMachine<BossEnemy>(this);
        stateMachine.Add<IdleState>((int)EnemyState.Idle);
        stateMachine.Add<ChaseState>((int)EnemyState.Chase);
        stateMachine.Add<AttackState>((int)EnemyState.Attack);
        stateMachine.Add<VigilanceState>((int)EnemyState.Vigilance);
        stateMachine.Add<SumonState>((int)EnemyState.Sumon);
        stateMachine.Add<DeadState>((int)EnemyState.Dead);
        stateMachine.Add<SkillState>((int)EnemyState.Skill);
        stateMachine.Onstart((int)EnemyState.Idle);
    }


    protected override void Update()
    {
        base.Update();
        if (currentHP <= 0)
        {
            stateMachine.ChangeState((int)EnemyState.Dead);
        }
        stateMachine.OnUpdate();
    }


    //=========================================
    // ▼攻撃・スキル関連（クールタイム対応部）
    //=========================================

    // 使用可能なスキルをランダムに取得
    private SkillData GetRandomAvailableSkill()
    {
        List<SkillData> available = new List<SkillData>();

        foreach (var s in skillList)
        {
            if (Time.time - s.lastUsedTime >= s.cooldown)
                available.Add(s);
        }

        if (available.Count == 0) return null;

        return available[Random.Range(0, available.Count)];
    }

    // 攻撃 or スキル実行
    public void TryDoAction()
    {
        // 通常攻撃間隔チェック
        if (Time.time - lastAttackTime < attackInterval)
            return;

        lastAttackTime = Time.time;

        // スキルを試す（例：30%）
        var skill = GetRandomAvailableSkill();
        if (skill != null && Probability(30))
        {
            UseSkill(skill);
            return;
        }

        // 通常攻撃へ
        stateMachine.ChangeState((int)EnemyState.Attack);
    }

    private void UseSkill(SkillData skill)
    {
        skill.lastUsedTime = Time.time;
        this.skills = skill.status;
        stateMachine.ChangeState((int)EnemyState.Skill);
    }


    //=========================================
    // ▼攻撃コライダー
    //=========================================
    public override void OnAttackSet()
    {
        attackColliders.ForEach(c => c.enabled = false);

        var state = animator.GetAnimatorTransitionInfo(0);
        foreach (var kv in colliderDict)
        {
            if (state.IsName(kv.Key))
            {
                kv.Value.enabled = true;
                break;
            }
        }
    }

    public override void OnAttackEnd() => attackColliders.ForEach(c => c.enabled = false);


    private void OnEffects()
    {
        var state = animator.GetAnimatorTransitionInfo(0);
        foreach (var kv in effectDict)
            kv.Value.SetActive(state.IsName(kv.Key) && state.normalizedTime < 1f);
    }


    //=========================================
    // ▼召喚
    //=========================================
    public override void OnSumon()
    {
        for (int i = 0; i < mobEnemy.Length; i++)
        {
            if (mobEnemy[i] == null) continue;

            float angle = Random.Range(-90, 90);
            Quaternion rot = Quaternion.Euler(0f, angle, 0f);
            Vector3 dir = rot * transform.forward;

            Vector3 spawnPos = transform.position + dir.normalized * 3;
            Instantiate(mobEnemy[i], spawnPos, Quaternion.identity);
        }
    }


    //=========================================
    // ▼各ステート
    //=========================================

    private class IdleState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnUpdate() =>
            StateMachine.ChangeState((int)EnemyState.Chase);
    }


    private class ChaseState : StateMachine<BossEnemy>.StateBase
    {
        NavMeshAgent nav;

        public override void OnStart()
        {
            nav = Owner.navMeshAgent;
            nav.isStopped = false;
        }

        public override void OnUpdate()
        {
            Vector3 playerpos = Owner.playerpos;
            nav.SetDestination(playerpos);

            if (Owner.Getdistance() <= Owner.AttackRange)
            {
                nav.isStopped = true;

                // 攻撃判断
                Owner.TryDoAction();
            }
        }
    }


    private class AttackState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.transform.LookAt(Owner.playerpos);
            Owner.navMeshAgent.isStopped = true;
        }

        public override void OnUpdate()
        {
            if (Owner.Getdistance() <= Owner.AttackRange)
            {
                Owner.TryDoAction();
            }
            else
            {
                StateMachine.ChangeState((int)EnemyState.Chase);
            }
        }
    }


    private class VigilanceState : StateMachine<BossEnemy>.StateBase
    {
        float time;
        float mTime;
        public float roamRadius = 5f;
        public float roamChangeInterval = 2f;

        private Vector3 roamTarget;
        private float roamTimer;

        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = false;
            time = 0;
            mTime = Random.Range(4, 6);
            PickNewRoamPosition();
        }

        public override void OnUpdate()
        {
            // 攻撃・スキル発動チェック
            Owner.TryDoAction();

            // 時間経過による遷移
            if (time > mTime)
            {
                time = 0;
                StateMachine.ChangeState((int)EnemyState.Sumon);
            }
            time += Time.deltaTime;

            float distance = Owner.Getdistance();

            if (distance < Owner.AttackRange)
            {
                Vector3 dir = (Owner.transform.position - Owner.playerpos).normalized;
                Vector3 retreatPos = Owner.player.transform.position + dir * Owner.AttackRange * 2;
                Owner.navMeshAgent.SetDestination(retreatPos);
            }
            else
            {
                roamTimer -= Time.deltaTime;
                if (roamTimer <= 0f)
                {
                    PickNewRoamPosition();
                }
                Owner.navMeshAgent.SetDestination(roamTarget);
            }

            Vector3 lookDir = Owner.playerpos - Owner.transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                Owner.transform.rotation =
                    Quaternion.Slerp(Owner.transform.rotation, Quaternion.LookRotation(lookDir), 0.1f);
            }
        }

        void PickNewRoamPosition()
        {
            roamTimer = roamChangeInterval;
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float r = Random.Range(0f, roamRadius);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * r, 0, Mathf.Sin(angle) * r);
            roamTarget = Owner.playerpos + offset;
        }
    }


    private class SumonState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;
        }

        public override void OnUpdate()
        {
            // 終了後の遷移
            Owner.TryDoAction();
        }
    }


    private class DeadState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnUpdate()
        {
            Owner.OnDead();
        }
    }


    private class SkillState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.Strength = Owner.skills.atk;
            Owner.AttackRange = Owner.skills.lenge;
        }

        public override void OnUpdate()
        {
            StateMachine.ChangeState((int)EnemyState.Vigilance);
        }
    }


    //=========================================
    // ▼被弾
    //=========================================
    public override int TakeDamage(DamageData dmg) =>
        base.TakeDamage(dmg);


    //=========================================
    // ▼Inspector GUI
    //=========================================
    public void DrawRunningStatusGUI()
    {
        EditorGUILayout.FloatField("現在のHP:", currentHP);
        EditorGUILayout.FloatField("HPの最大値:", MaxHP);
        EditorGUILayout.FloatField("移動速度:", MoveSpeed);
        EditorGUILayout.FloatField("攻撃力:", Strength);
    }

    public SerializedObject GetSerializedBaseStatus()
    {
        if (BossStatus == null) return null;

        if (seliarizeBossStatus == null || seliarizeBossStatus.targetObject != BossStatus)
            seliarizeBossStatus = new SerializedObject(BossStatus);

        return seliarizeBossStatus;
    }
}
