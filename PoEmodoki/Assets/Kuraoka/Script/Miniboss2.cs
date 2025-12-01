using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.UI.GridLayoutGroup;
using Unity.VisualScripting;
using UnityEditor;

public class Miniboss2 : Enemy, IStatusView
{
    [SerializeField] SkillStatus skills;
    [SerializeField] EnemyStatus BossStatus;
    private SerializedObject seliarizeBossStatus;       //S0をキャッシュする用
    StateMachine<Miniboss2> stateMachine;
    [SerializeField] GameObject[] mobEnemy;

    [SerializeField] private List<string> attackStates;
    [SerializeField] private List<GameObject> effects;
    [SerializeField] private List<Collider> attackColliders;
    private Dictionary<string, Collider> colliderDict;
    private Dictionary<string, GameObject> effectDict;
    private enum EnemyState
    {
        Idle,//待機
        Chase,//移動、追尾
        Vigilance,//攻撃前の警戒ステート
        Attack,//攻撃仮
        Hit,//被弾
        Skill,//特殊能力
        Dead,//死亡
        Rotate,//回転
    }



    private void Awake()
    {
        //攻撃エフェクトとコライダーの生成  
        effectDict = new Dictionary<string, GameObject>();
        for (int i = 0; i < Mathf.Min(attackStates.Count, effects.Count); i++)
        {
            effectDict[attackStates[i]] = effects[i];
            effects[i].SetActive(false);
            if (i < attackColliders.Count && attackColliders[i] != null)
                colliderDict[attackStates[i]] = attackColliders[i];
        }

        foreach (var c in attackColliders) c.enabled = false;
        colliderDict = new Dictionary<string, Collider>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        MaxHP = BossStatus.EnemyHp;
        currentHP = MaxHP;
        Strength = BossStatus.EnemyAtk;
        AttackSpeed = BossStatus.EnemyAtkSpeed;
        AttackRange = BossStatus.EnemyLength;
        MoveSpeed = BossStatus.EnemySpeed;
        fov = BossStatus.EnemyFov;
        name = BossStatus.EnemyName;
        navMeshAgent = GetComponent<NavMeshAgent>();

        stateMachine = new StateMachine<Miniboss2>(this);
        stateMachine.Add<IdleState>((int)EnemyState.Idle);
        stateMachine.Add<ChaseState>((int)EnemyState.Chase);
        stateMachine.Add<VigilanceState>((int)EnemyState.Vigilance);
        stateMachine.Add<SkillState>((int)EnemyState.Skill);
        stateMachine.Add<DeadState>((int)EnemyState.Dead);
        stateMachine.Onstart((int)EnemyState.Idle);
    }

    // Update is called once per frame
    public override void Update()
    {
        //死亡判定
        base.Update();
        if (currentHP <= 0)
        {
            stateMachine.ChangeState((int)EnemyState.Dead);
        }
        //stateMachine.OnUpdate();
    }

    public override void OnAttackSet()
    {
        //攻撃判定
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
    //攻撃終了判定
    public override void OnAttackEnd() => attackColliders.ForEach(c => c.enabled = false);
    //エフェクトかんれん
    private void OnEffects()
    {
        var state = animator.GetAnimatorTransitionInfo(0);
        foreach (var kv in effectDict)
            kv.Value.SetActive(state.IsName(kv.Key) && state.normalizedTime < 1f);
    }
    public override void OnSumon()
    {
        for (int i = 0; i < mobEnemy.Length; i++)
        {
            if (mobEnemy == null) continue;//nullかチェック
            float angle = Random.Range(-90, 90);//召喚範囲
            Quaternion rot = Quaternion.Euler(0f, angle, 0f);
            Vector3 dir = rot * transform.forward;
            Vector3 spawnPos = transform.position + dir.normalized * 3;
            Instantiate(mobEnemy[i], spawnPos, Quaternion.identity);
        }
    }

    //各ステートの定義
    private class IdleState : StateMachine<Miniboss2>.StateBase
    {
        float cDis;
        public override void OnStart()
        {
            //Owner.animator.SetTrigger("Idle");
            cDis = Owner.Distance;//プレイヤーを見つけられる距離
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

    private class ChaseState : StateMachine<Miniboss2>.StateBase
    {
        NavMeshAgent navMeshAgent;
        public override void OnStart()
        {
            //Owner.animator.SetTrigger("Chase");
            navMeshAgent = Owner.navMeshAgent;
            navMeshAgent.isStopped = false;
        }

        public override void OnUpdate()
        {
            Vector3 playerpos = Owner.playerpos.transform.position;
            if (Owner.Getdistance() <= Owner.AttackRange)

            {
                //確率で各ステートに移行
                if (Probability(60)) { StateMachine.ChangeState((int)EnemyState.Attack); }
                if (Probability(20)) { StateMachine.ChangeState(((int)EnemyState.Rotate)); }
                if (Probability(20)) { StateMachine.ChangeState(((int)EnemyState.Skill)); }

            }
        }
        public override void OnEnd()
        {
            //Owner.animator.ResetTrigger("Chase");
        }
    }
    private class VigilanceState : StateMachine<Miniboss2>.StateBase
    {   
        float time;
        float mTime;

        public float roamRadius = 5f;      // プレイヤーを中心とした円の半径
        public float roamChangeInterval = 2f; // ランダム位置を更新する間隔

        private Vector3 roamTarget;        // 今の円内ターゲット位置
        private float roamTimer;
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = false;
            //Owner.animator.SetTrigger("Idle");
            time = 0;
            mTime = Random.Range(4, 6);
            PickNewRoamPosition();
        }
        public override void OnUpdate()
        {
            if (time > mTime)
            {
                //確率で各ステートに移行
                time = 0;
                if (Probability(60)) { StateMachine.ChangeState((int)EnemyState.Skill); }
                if (Probability(40)) { StateMachine.ChangeState((int)EnemyState.Rotate); }
            }
            time += Time.deltaTime;

            float distance = Owner.Getdistance();

            if (distance < Owner.AttackRange)
            {
                Vector3 dir = (Owner.transform.position - Owner.playerpos.transform.position).normalized;
                Vector3 retreatPos = Owner.playerpos.transform.position + dir * Owner.AttackRange * 2;
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

            Vector3 lookDir = Owner.playerpos.transform.position - Owner.transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, Quaternion.LookRotation(lookDir), 0.1f);
            }
        }
        public override void OnEnd()
        {
            //Owner.animator.ResetTrigger("Idle");
        }
        void PickNewRoamPosition()
        {
            roamTimer = roamChangeInterval;
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float r = Random.Range(0f, roamRadius);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * r, 0, Mathf.Sin(angle) * r);
            roamTarget = Owner.playerpos.transform.position + offset;
        }
    }
    private class SkillState : StateMachine<Miniboss2>.StateBase
    {
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;
            
            //Owner.animator.SetTrigger("Sumon");

        }
        public override void OnUpdate()
        {
            if (Owner.AnimationEnd("Sumon"))
            {
                //確率で各ステートに移行
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

                    }
                }
            }
        }
        public override void OnEnd()
        {
            //Owner.animator.ResetTrigger("Sumon");
        }
    }
    private class DeadState : StateMachine<Miniboss2>.StateBase
    {
        public override void OnStart()
        {
            // Owner.animator.SetTrigger("Dead");
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
            // Owner.animator.ResetTrigger("Dead");
        }
    }

    //被弾処理
    public override int TakeDamage(DamageData dmg)
    {
        int damageTaken = base.TakeDamage(dmg);

        if (currentHP <= 0)
        {

        }
        return damageTaken;
    }

    public void DrawRunningStatusGUI()
    {
        EditorGUILayout.FloatField("現在のHP:", currentHP);
        EditorGUILayout.FloatField("HPの最大値:", MaxHP);
        EditorGUILayout.FloatField("移動速度:", MoveSpeed);
        EditorGUILayout.FloatField("攻撃力:", Strength);
    }

    public SerializedObject GetSerializedBaseStatus()
    {
        if (BossStatus == null)
        {
            return null;
        }

        if (seliarizeBossStatus == null || seliarizeBossStatus.targetObject != BossStatus)
        {
            seliarizeBossStatus = new SerializedObject(BossStatus);
        }
        return seliarizeBossStatus;
    }

}
