using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.UI.GridLayoutGroup;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;

public class MiniBoss1 : Enemy

{
    [SerializeField] EnemyStatus Miniboss1Status;
    private SerializedObject serializeminibossStatus;
    StateMachine<MiniBoss1> statemachine;
    [SerializeField] private List<string> attackStates;
    [SerializeField] private List<GameObject> effects;
    [SerializeField] private List<Collider> attackColliders
    private Dictionary<string, Collider> colliderDict;
    private Dictionary<string, GameObject> effectDict;


    private enum EnemyState
    {
        Idle,
        Chase,
        Vigilance,
        Attack,
        Hit,
        Sumon,
        Dead,
        Rotate,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    private void Start()
    {
        MaxHP = Miniboss1Status.EnemyHp;
        currentHP = MaxHP;
        Strength = Miniboss1Status.EnemyAtk;
        AttackSpeed = Miniboss1Status.EnemyAtkSpeed;
        AttackRange = Miniboss1Status.EnemyLength;
        MoveSpeed = Miniboss1Status.EnemySpeed;
        fov = Miniboss1Status.EnemyFov;
        name = Miniboss1Status.EnemyName;
        navMeshAgent = GetComponent<NavMeshAgent>();

        stateMachine = new StateMachine<BossEnemy>(this);
        stateMachine.Add<IdleState>((int)EnemyState.Idle);
        stateMachine.Add<ChaseState>((int)EnemyState.Chase);
        stateMachine.Add<VigilanceState>((int)EnemyState.Vigilance);
        stateMachine.Add<SumonState>((int)EnemyState.Sumon);
        stateMachine.Add<DeadState>((int)EnemyState.Dead);
        stateMachine.Onstart((int)EnemyState.Idle);
    }

    protected override void Update()
    {
        base.Update();
        if(currentHP<=0)
        {
            statemachine.ChangeState((int)EnemyState.Dead);

        }
        statemachine.OnUpdate();
    }
}
