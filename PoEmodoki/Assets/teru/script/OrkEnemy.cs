using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class OrkEnemy : Enemy
{
    StateMachine<OrkEnemy> stateMachine;
    [SerializeField] EnemyStatus Status;
#if UNITY_EDITOR
    private SerializedObject seliarizeZonbiStatus;       //S0をキャッシュする用
#endif
    [SerializeField] SkillStatus skills;
    //protected NavMeshAgent navMeshAgent;
    [SerializeField] private List<GameObject> effects;
    [SerializeField] private List<Collider> attackColliders;
    private Dictionary<string, Collider> colliderDict;
    private Dictionary<string, GameObject> effectDict;
    private SkillStatus currentSkill;

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
        stateMachine = new StateMachine<OrkEnemy>(this);
        stateMachine.Add<IdleState>((int)State.Idle);
        stateMachine.Add<PatrolState>((int)State.Patrol);
        stateMachine.Add<ChaseState>((int)State.Chase);
        stateMachine.Add<AttackState>((int)State.Attack);
        stateMachine.Add<AttackIntervalState>((int)State.AttackInt);
        stateMachine.Add<HitState>((int)State.Hit);
        stateMachine.Add<DeadState>((int)State.Dead);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        stateMachine.OnUpdate();
    }

    private class IdleState : StateMachine<OrkEnemy>.StateBase
    {
        float cDis;
        public override void OnStart()
        {

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
            //Owner.enemyAnimation.ResetTrigger("Idle");
        }
    }
    private class PatrolState : StateMachine<OrkEnemy>.StateBase
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
            //Owner.enemyAnimation.SetTrigger("Walk");
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
            //Owner.enemyAnimation.ResetTrigger("Walk");
        }
    }

    private class ChaseState : StateMachine<OrkEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = false;
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
            //Owner.animator.ResetTrigger("Chase");
        }
    }

    private class AttackState : StateMachine<OrkEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.navMeshAgent.isStopped = true;
            Owner.currentSkill = Owner.skills;

            Owner.Strength = (int)Owner.skills.atk;

            // プレイヤーに向ける
            Vector3 lookPos = Owner.player.transform.position;
            lookPos.y = Owner.transform.position.y;
            Owner.transform.LookAt(lookPos);

            // スキルアニメ再生
            // Owner.animator.SetTrigger("Skill");


            // ================================
            // スキル発射
            // ================================
            FireSkill();

        }

        private void FireSkill()
        {
            if (Owner.skills.effect == null)
            {
                Debug.LogWarning("Skill prefab is null for skill: " + Owner.skills.name);
                return;
            }

            // 発射位置（少し前方 & 上）
            Vector3 spawnPos =
                Owner.transform.position +
                Owner.transform.forward * 1.5f +
                Vector3.up * 1.2f;
            GameObject proj =
                GameObject.Instantiate(Owner.skills.effect, spawnPos, Owner.transform.rotation);

            // Rigidbody があれば速度を付与
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Owner.transform.forward * Owner.skills.speed;
            }

            // ================================
            // 射程による自動消滅
            // ================================
            // lenge と length のどちらか大きい方を射程とする
            float range = Mathf.Max(Owner.skills.lenge, Owner.skills.length);
            if (range > 0 && Owner.skills.speed > 0)
            {
                float lifetime = range / Owner.skills.speed;
                GameObject.Destroy(proj, lifetime);
            }
        }

        public override void OnUpdate()
        {


        }
        public override void OnEnd()
        {
            //Owner.enemyAnimation.ResetTrigger("Combo");
        }
    }
    private class AttackIntervalState : StateMachine<OrkEnemy>.StateBase
    {
        float time;
        public override void OnStart()
        {

        }
        public override void OnUpdate()
        {
            time += Time.deltaTime;
            if (time > Owner.AttackSpeed) { StateMachine.ChangeState((int)State.Idle); time = 0; }
        }
        public override void OnEnd()
        {
            //Owner.enemyAnimation.ResetTrigger("Combo");
        }
    }


    private class HitState : StateMachine<OrkEnemy>.StateBase
    {
        public override void OnStart()
        {

        }
        public override void OnUpdate()
        {


        }
        public override void OnEnd()
        {
            //Owner.enemyAnimation.ResetTrigger("Combo");
        }
    }

    private class DeadState : StateMachine<OrkEnemy>.StateBase
    {
        public override void OnStart()
        {
            Owner.transform.LookAt(Owner.player.transform.position);
            //Owner.enemyAnimation.SetTrigger("Combo");
            Owner.navMeshAgent.isStopped = true;
        }
        public override void OnUpdate()
        {


        }
        public override void OnEnd()
        {
            //Owner.enemyAnimation.ResetTrigger("Combo");
        }
    }
}
