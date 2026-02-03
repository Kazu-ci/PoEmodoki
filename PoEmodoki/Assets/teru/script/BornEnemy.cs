using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BornEnemy : Enemy
{
    StateMachine<BornEnemy> stateMachine;
    [SerializeField] EnemyStatus Status;
#if UNITY_EDITOR
    private SerializedObject seliarizeZonbiStatus;       //S0をキャッシュする用
#endif

    //protected NavMeshAgent navMeshAgent;
    [SerializeField] private List<GameObject> effects;
    [SerializeField] private List<Collider> attackColliders;
    private Dictionary<string, Collider> colliderDict;
    private Dictionary<string, GameObject> effectDict;

    //private SkillStatus currentSkill;
    [SerializeField] int dropcount = 1;//ドロップするソウルの数
    [SerializeField] private GameObject soulprefab;//ドロップさせるソウルの種類
    //アニメーション関係
    public readonly int AnimIdle = Animator.StringToHash("Idle");
    public readonly int AnimChase = Animator.StringToHash("Chase");
    public readonly int AnimWalk = Animator.StringToHash("Walk");
    public readonly int AnimSkill = Animator.StringToHash("Skill");
    public readonly int AnimHit = Animator.StringToHash("Hit");
    public readonly int AnimAttack = Animator.StringToHash("Attack");
    public readonly int AnimDead = Animator.StringToHash("Dead");

    protected enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        AttackInt,
        Hit,
        Dead
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        MaxHP = Status.EnemyHp;
        currentHP = MaxHP;
        Strength = Status.EnemyAtk;
        AttackSpeed = Status.EnemyAtkSpeed;
        AttackRange = Status.EnemyLength;
        MoveSpeed = Status.EnemySpeed;
        lookPlayerDir = Status.EnemySens;
        fov = Status.EnemyFov;
        name = Status.EnemyName;
        stateMachine = new StateMachine<BornEnemy>(this);
        stateMachine.Add<IdleState>((int)State.Idle);
        stateMachine.Add<PatrolState>((int)State.Patrol);
        stateMachine.Add<ChaseState>((int)State.Chase);
        stateMachine.Add<AttackState>((int)State.Attack);
        stateMachine.Add<AttackIntervalState>((int)State.AttackInt);
        stateMachine.Add<HitState>((int)State.Hit);
        stateMachine.Add<DeadState>((int)State.Dead);
        stateMachine.Onstart((int)State.Idle);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        stateMachine.OnUpdate();
    }
    public override void OnAttackSet()
    => attackColliders.ForEach(c => c.enabled = true);

    public override void OnAttackEnd() => attackColliders.ForEach(c => c.enabled = false);
    private class IdleState : StateMachine<BornEnemy>.StateBase
    {
        float cDis;
        public override void OnStart()
        {
            Owner.animator.CrossFade(Owner.AnimIdle, 0.1f);
        }
        public override void OnUpdate()
        {
            float playerDis = Owner.Getdistance();
            var playerDir = Owner.playerpos - Owner.transform.position;
            var angle = Vector3.Angle(Owner.transform.forward, playerDir);
            if (playerDis <= cDis && angle <= Owner.fov) { StateMachine.ChangeState((int)State.Chase); }
            else { StateMachine.ChangeState((int)State.Patrol); }
        }
        public override void OnEnd()
        {

        }
    }
    private class PatrolState : StateMachine<BornEnemy>.StateBase
    {
        float cDis;
        Vector3 endPos;
        Vector3 startPos;
        bool goingToEnd = true;
        bool firstInit = true;
        public override void OnStart()
        {

            //Owner.ChangeTexture(0);
            Owner.navMeshAgent.isStopped = false;
            if (firstInit)
            {
                startPos = Owner.transform.position;
                endPos = Owner.GetRandomNavMeshPoint(startPos, 10f);
                firstInit = false;
            }

            cDis = Owner.lookPlayerDir;
        }
        public override void OnUpdate()
        {
            Owner.animator.CrossFade(Owner.AnimIdle, 0.1f);
            float playerDis = Owner.Getdistance();
            var playerDir = Owner.playerpos - Owner.transform.position;
            var angle = Vector3.Angle(Owner.transform.forward, playerDir);
            if (playerDis <= cDis && angle <= Owner.fov)            // プレイヤー検出
            {
                StateMachine.ChangeState((int)State.Chase);
                return;
            }
            // パトロール
            Vector3 targetPos = goingToEnd ? endPos : startPos;
            Owner.navMeshAgent.SetDestination(targetPos);
            // 到着判定
            if (!Owner.navMeshAgent.pathPending && Owner.navMeshAgent.remainingDistance <= Owner.navMeshAgent.stoppingDistance)
            {
                goingToEnd = !goingToEnd;
                StateMachine.ChangeState((int)State.Idle);
            }
        }
        public override void OnEnd()
        {

        }
    }

    private class ChaseState : StateMachine<BornEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = false;
            Owner.animator.CrossFade(Owner.AnimChase, 0.1f);
        }
        public override void OnUpdate()
        {
            if (Owner.Getdistance() <= Owner.AttackRange)
            {
                StateMachine.ChangeState((int)State.Attack);
                Owner.navMeshAgent.isStopped = true;
            }
            if (Owner.Getdistance() >= Owner.lookPlayerDir) { StateMachine.ChangeState((int)State.Idle); }
            Vector3 playerPos = Owner.playerpos;
            Owner.navMeshAgent.SetDestination(playerPos);
        }
        public override void OnEnd()
        {

        }
    }

    private class AttackState : StateMachine<BornEnemy>.StateBase
    {
        private bool _isSkillPlaying;
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;
            Owner.IsAttacking = true;
            Owner.OnAttackSet();

            // 向きを合わせる
            Vector3 lookPos = Owner.playerpos;
            lookPos.y = Owner.transform.position.y;
            Owner.transform.LookAt(lookPos);
            // スキルアニメ再生
            Owner.animator.CrossFade(Owner.AnimSkill, 0.1f);


            // ================================
            // スキル発射
            // ================================
            Owner.UseSkill(Owner.skills.Count);

            _isSkillPlaying = true;
        }



        public override void OnUpdate()
        {
            {
                if (!_isSkillPlaying) return;

                AnimatorStateInfo info =
                    Owner.animator.GetCurrentAnimatorStateInfo(0);

                // Skillアニメが再生完了したら遷移
                if (info.shortNameHash == Owner.AnimSkill &&
                    info.normalizedTime >= 1.0f)
                {
                    _isSkillPlaying = false;
                    Owner.stateMachine.ChangeState((int)State.AttackInt);
                }
            }
        }
        public override void OnEnd()
        {
           Owner.OnAttackEnd();
        }
    }
    private class AttackIntervalState : StateMachine<BornEnemy>.StateBase
    {
        float time;
        public override void OnStart()
        {
            Owner.animator.CrossFade(Owner.AnimIdle, 0.1f);
        }
        public override void OnUpdate()
        {
            time += Time.deltaTime;
            if (time > Owner.AttackSpeed) { StateMachine.ChangeState((int)State.Idle); time = 0; }
        }
        public override void OnEnd()
        {

        }
    }


    private class HitState : StateMachine<BornEnemy>.StateBase
    {
        float timer;
        const float hitDuration = 0.5f;

        public override void OnStart()
        {
            timer = 0;
            Owner.animator.CrossFade(Owner.AnimHit, 0.1f);
        }
        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if (timer >= hitDuration)
            {
                StateMachine.ChangeState((int)State.Idle);
            }

        }
        public override void OnEnd()
        {

        }
    }

    private class DeadState : StateMachine<BornEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.transform.LookAt(Owner.playerpos);
            Owner.animator.CrossFade(Owner.AnimDead, 0.1f);

            Owner.navMeshAgent.isStopped = true;
        }
        public override void OnUpdate()
        {


        }
        public override void OnEnd()
        {

        }
    }
    protected override void Drop()
    {
        if (soulprefab == null)
        {
            return;
        }
        for (int i = 0; i < dropcount; ++i)
        {
            Instantiate(soulprefab, thisobj.transform.position, Quaternion.identity);
        }
    }

}
