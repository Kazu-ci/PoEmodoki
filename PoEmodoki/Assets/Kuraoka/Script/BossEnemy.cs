using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.UI.GridLayoutGroup;
using Unity.VisualScripting;
using System;
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
    [SerializeField] int dropcount = 1;//ドロップするソウルの数
    [SerializeField] private GameObject soulprefab;//ドロップさせるソウルの種類

    private Dictionary<string, Collider> colliderDict;
    private Dictionary<string, GameObject> effectDict;

#if UNITY_EDITOR
    private SerializedObject seliarizeBossStatus;
#endif

    public readonly int AnimIdle = Animator.StringToHash("Idle");
    public readonly int AnimVigi = Animator.StringToHash("RunFowrard");
    public readonly int AnimSkill = Animator.StringToHash("Ability");
    public readonly int AnimHit = Animator.StringToHash("TakingDaamage");
    public readonly int AnimAttack = Animator.StringToHash("Attack");
    public readonly int AnimSumon = Animator.StringToHash("Stun");
    public readonly int AnimDead = Animator.StringToHash("Death");

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
    => attackColliders.ForEach(c => c.enabled = true);

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
            float angle = UnityEngine.Random.Range(-90, 90);
            Quaternion rot = Quaternion.Euler(0, angle, 0);
            Vector3 dir = rot * transform.forward;
            Vector3 spawnPos = transform.position + dir.normalized * 5;
            Instantiate(mobEnemy[i], spawnPos, Quaternion.identity);
        }
    }


    private class IdleState : StateMachine<BossEnemy>.StateBase
    {
        float cDis;
        public override void OnStart()
        {
            Owner.animator.CrossFade(Owner.AnimIdle, 0.1f);
            cDis = Owner.lookPlayerDir;
        }
        public override void OnUpdate()
        {
            StateMachine.ChangeState((int)EnemyState.Chase);
        }
        public override void OnEnd()
        {
          
        }
    }

    private class ChaseState : StateMachine<BossEnemy>.StateBase
    {
        NavMeshAgent navMeshAgent;
        public override void OnStart()
        {
           
            navMeshAgent = Owner.navMeshAgent;

            navMeshAgent.isStopped = false;
            Owner.animator.CrossFade(Owner.AnimVigi, 0.1f);
        }
        public override void OnUpdate()
        {
            Vector3 playerPos = Owner.playerpos;
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
          
        }
    }
    private class VigilanceState : StateMachine<BossEnemy>.StateBase
    {
        float time;
        float mTime;

        public float roamRadius = 2f;      // プレイヤーを中心とした円の半径
        public float roamChangeInterval = 2f;// ランダム位置を更新する間隔

        private Vector3 roamTarget;      // 今の円内ターゲット位置
        private float roamTimer;
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = false;
            Owner.animator.CrossFade(Owner.AnimIdle, 0.1f);
            time = 0;
            mTime = UnityEngine.Random.Range(4, 6);
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
                Vector3 dir = (Owner.transform.position - Owner.playerpos).normalized;
                Vector3 retreatPos = Owner.playerpos + dir * Owner.AttackRange * 2;
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

            Vector3 lookDir = Owner.playerpos - Owner.transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, Quaternion.LookRotation(lookDir), 0.1f);
            }
        }
        public override void OnEnd()
        {
           
        }
        void PickNewRoamPosition()
        {
            roamTimer = roamChangeInterval;
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float r = UnityEngine.Random.Range(0f, roamRadius);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * r, 0, Mathf.Sin(angle) * r);
            roamTarget = Owner.playerpos + offset;
        }
    }

    private class AttackState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.transform.LookAt(Owner.playerpos);
            Owner.animator.CrossFade(Owner.AnimAttack, 0.1f);
            Owner.navMeshAgent.isStopped = true;
            Owner.OnAttackSet();
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
            Owner.OnAttackEnd();
        }
    }
    private class RotateState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;
            Owner.animator.CrossFade(Owner.AnimAttack, 0.1f);
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
           
        }
    }
    private class SumonState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;
            Owner.animator.CrossFade(Owner.AnimSumon, 0.1f);

        }
        public override void OnUpdate()
        {
            if (Owner.AnimationEnd("Stun"))
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
            
        }
    }
    private class SkillState : StateMachine<BossEnemy>.StateBase
    {
        private SkillStatus skill;

        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;

            // ボスだけ複数スキルから選ぶ
            int index = UnityEngine.Random.Range(0, Owner.skills.Count);
           

            // 向きを合わせる
            Vector3 lookPos = Owner.playerpos;
            lookPos.y = Owner.transform.position.y;
            Owner.transform.LookAt(lookPos);

            Owner.animator.CrossFade(Owner.AnimSkill, 0.1f);


            Owner.UseSkill(index);
        }

        public override void OnUpdate()
        {
            if (Owner.AnimationEnd("Ability"))
            {
                StateMachine.ChangeState((int)EnemyState.Chase);
            }
        }
        public override void OnEnd()
        {

        }
    }
    private class StiffnessState : StateMachine<BossEnemy>.StateBase
    {
        float time;
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;
            Owner.animator.CrossFade(Owner.AnimIdle, 0.1f);
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
           
        }

    }

    private class HitState : StateMachine<BossEnemy>.StateBase
    {
        float timer = 0;
        const float hitDuration = 0.5f;

        public override void OnStart()
        {
            Owner.animator.CrossFade(Owner.AnimHit, 0.1f);
        }
        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if (timer >= hitDuration)
            {
                StateMachine.ChangeState((int)EnemyState.Idle);
            }
        }
        public override void OnEnd()
        {
          
        }
    }
    private class DeadState : StateMachine<BossEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.animator.CrossFade(Owner.AnimDead, 0.1f);
        }
        public override void OnUpdate()
        {
            if (Owner.AnimationEnd("Death"))
            {
                Owner.OnDead();
            }
        }
        public override void OnEnd()
        {
           
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

    protected override void Drop()
    {
        if (soulprefab == null)
        {
            return;
        }
        for(int i = 0;i<dropcount;++i)
        {
            Instantiate(soulprefab, thisobj.transform.position, Quaternion.identity);
        }
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
