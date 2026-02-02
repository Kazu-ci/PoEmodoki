using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;




public class PlayerCon : MonoBehaviour,IStatusView
{
    [SerializeField]PlayerStatus player;
#if UNITY_EDITOR
    private SerializedObject sPlayerStatus;
#endif
    //プレイヤーのステータス管理
    [SerializeField]PlayerInput PlayerInput;
    //プレイヤーの座標管理
    [SerializeField]PlayerAnchor playerAnchor;
    //[SerializeField]SkillStatus skill;//仮
    [SerializeField]BossEnemy bossEnemy;
    //インタラクト可能な半径
    [SerializeField] private float InteractRange = 3f;
    //インタラクト対象レイヤー
    [SerializeField] private LayerMask InteractLayer;
    InputAction move;
    InputAction skill1,skill2,skill3,skill4;
    //ロックオン
    [SerializeField] private GameObject lockonTarget;
    [SerializeField] private GameObject playerObj;
    public List<SkillStatus> mySkills = new List<SkillStatus>();
    public List<SkillStatus> allskill = new List<SkillStatus>();
    private Vector2 moveVec = default;
    private IUseSkill[] skills = new IUseSkill[10];
    bool OnSkill = false;
    bool OnAttack = false;
    //通常攻撃の当たり判定
    public Transform atkPoint;
    public LayerMask enemyLayer;
    //ステータス
    int MaxHP;
    int HP;
    int Defense;
    int MP;
    int Atk;
    float MoveSpeed;
    float AtkSpeed;
    float CastSpeed;
    float Length;
    float ElementDefense;
    float Critical;

    //以下アニメーション関連
    public Animator anim;

    //ハッシュ化
    public readonly int AnimIdle = Animator.StringToHash("Idle");
    public readonly int AnimRun = Animator.StringToHash("RunForward");
    public readonly int AnimAttack = Animator.StringToHash("Attack");
    public readonly int AnimSkill = Animator.StringToHash("Ability");
    public readonly int AnimStun = Animator.StringToHash("Stun");
    public readonly int AnimTakingDamage = Animator.StringToHash("TakingDamage");
    public readonly int AnimDeath = Animator.StringToHash("Death");

    StateMachine<PlayerCon> stateMachine;
    enum state
    {
        Idol,
        Move,
        Attack,
        Hit,
        SkillAttack,
        Dead,
        pose,

    }

    private void Awake()
    {
        MaxHP = player.PlayerHp;
        HP = MaxHP;
        Defense = player.PlayerDefense;
        MP = player.PlayerMp;
        MoveSpeed = player.PlayerSpeed;
        AtkSpeed = player.PlayerAtkSpeed;
        CastSpeed = player.PlayerCastSpeed;
        Length = player.PlayerLength;
        ElementDefense = player.PlayerElementDefense;
        Critical = player.PlayerCritical;
        Atk = player.PlayerAtk;


        move = PlayerInput.actions["Move"];
        skill1 = PlayerInput.actions["Skill1"];
        skill2 = PlayerInput.actions["Skill2"];
        skill3 = PlayerInput.actions["Skill3"];
        skill4 = PlayerInput.actions["Skill4"];


        stateMachine = new StateMachine<PlayerCon>(this);
        stateMachine.Add<MoveState>((int)state.Move);
        stateMachine.Add<IdolState>((int)state.Idol);
        stateMachine.Add<AttackState>((int)state.Attack);
        stateMachine.Add<HitState>((int)state.Hit);
        stateMachine.Add<SkillAttackState>((int)state.SkillAttack);
        stateMachine.Add<DeadState>((int)state.Dead);
        stateMachine.Onstart((int)state.Idol);
        //デバッグ用スキル
        //mySkills.Add(skill);
        
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (HP <= 0) { stateMachine.ChangeState((int)state.Dead); }
        stateMachine.OnUpdate();
    }
    public void Getvalue(float s)
    {
        MoveSpeed = s;
    }
    public class MoveState : StateMachine<PlayerCon>.StateBase
    {
        public override void OnStart()
        {
            Debug.Log("Move");
            Owner.anim.CrossFade(Owner.AnimRun, 0.1f);
        }
        public override void OnUpdate()
        {
            Vector3 direction = new Vector3(Owner.moveVec.x,0, Owner.moveVec.y);
            //移動処理
            if (direction != Vector3.zero)
            {
                Owner.gameObject.transform.Translate
                (new Vector3(Owner.moveVec.x, 0, Owner.moveVec.y) * Owner.MoveSpeed * Time.deltaTime, Space.World);
                //回転処理
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, targetRotation, 10f * Time.deltaTime);
            }

            if (Owner.move.ReadValue<Vector2>() == new Vector2(0, 0))
            {
                StateMachine.ChangeState((int)state.Idol);
            }
            if (Owner.OnAttack)
            {
                StateMachine.ChangeState((int)state.Attack);
            }
            else if (Owner.OnSkill)
            {
                StateMachine.ChangeState((int)state.SkillAttack);
            }

        }
        public override void OnEnd()
        {
            
        }
    }

    public class IdolState : StateMachine<PlayerCon>.StateBase
    {
        public override void OnStart()
        {
            Debug.Log("Idol");
            Owner.anim.CrossFade(Owner.AnimIdle, 0.1f);
        }
        public override void OnUpdate()
        {
            if(Owner.moveVec != Vector2.zero)
            {
                StateMachine.ChangeState((int)state.Move);
            }
            else if(Owner.OnSkill)
            {
                StateMachine.ChangeState((int)state.SkillAttack);
            }
            else if (Owner.OnAttack)
            {
                StateMachine.ChangeState((int)state.Attack);
            }
        }
        public override void OnEnd()
        {

        }
    }

    public class AttackState : StateMachine<PlayerCon>.StateBase
    {
        private bool attackHit = false;
        public override void OnStart()
        {
            Debug.Log("Attack");
            Owner.anim.CrossFade(Owner.AnimAttack, 0.1f);
            attackHit = false;
        }
        public override void OnUpdate()
        {
            AnimatorStateInfo stateInfo = Owner.anim.GetCurrentAnimatorStateInfo(0);

            float hitTime = stateInfo.normalizedTime;
            if(hitTime >= 0.2f && hitTime <= 0.7f)
            {
                if(!attackHit)
                {
                    hitCheckColl();
                }
            }

            if(hitTime >= 1.0f)
            {
                StateMachine.ChangeState((int)(state.Idol));
                //if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
                //{
                //    if (Owner.OnAttack == false)
                //    {
                //        StateMachine.ChangeState((int)(state.Idol));
                //    }
                //}
            }
        }
        public override void OnEnd()
        {

        }
        private void hitCheckColl()
        {
            float dagger = 1.2f;
            

            Collider[] hitEnemy = Physics.OverlapSphere(Owner.atkPoint.position, dagger, Owner.enemyLayer);
            foreach (var enemy in hitEnemy)
            {
                //ここでダメージ処理

                Debug.Log("Hit");

                attackHit = true;

            }

        }
    }

    public class HitState : StateMachine<PlayerCon>.StateBase
    {
        public override void OnStart()
        {
            Owner.anim.CrossFade(Owner.AnimTakingDamage, 1.0f);
        }
        public override void OnUpdate()
        {
            AnimatorStateInfo stateInfo = Owner.anim.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("TakingDamage") && stateInfo.normalizedTime >= 1.0f)
            {
                    StateMachine.ChangeState((int)(state.Idol));   
            }
        }
        public override void OnEnd()
        {

        }
    }

    public class SkillAttackState : StateMachine<PlayerCon>.StateBase
    {
        public override void OnStart()
        {
            Debug.Log("SkillAttack");
            Owner.anim.CrossFade(Owner.AnimSkill, 0.1f);
        }
        public override void OnUpdate()
        {
            AnimatorStateInfo stateInfo = Owner.anim.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Ability") && stateInfo.normalizedTime >= 1.0f)
            {
                if (Owner.OnSkill == false)
                {
                    StateMachine.ChangeState((int)(state.Idol));
                }
            }
            
        }
        public override void OnEnd()
        {
            
        }
    }

    public class DeadState : StateMachine<PlayerCon>.StateBase
    {
        public override void OnStart()
        {
            Debug.Log("Dead");
            Owner.anim.CrossFade(Owner.AnimDeath, 0.1f);
        }
        public override void OnUpdate()
        {

        }
        public override void OnEnd()
        {

        }
    }

    public void MovePlayer(InputAction.CallbackContext context)
    {
        moveVec = context.ReadValue<Vector2>();
    }
    //デバッグ用スキル
    public void OnSkillQ(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            OnSkill = true;
            UseSkill(0);
            skills[0]?.UseSkill(this);
        }
        else if(context.canceled)
        {
            OnSkill = false;
        }
    }
    public void OnSkillE(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSkill = true;
            UseSkill(1);
            skills[1]?.UseSkill(this);
        }
        else if (context.canceled)
        {
            OnSkill = false;
        }
    }
    public void OnSkillR(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSkill = true;
            UseSkill(2);
            skills[2]?.UseSkill(this);
        }
        else if (context.canceled)
        {
            OnSkill = false;
        }
    }
    public void OnSkillC(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            OnSkill = true;

            UseSkill(3);
            skills[3]?.UseSkill(this);
        }
        else if (context.canceled)
        {
            OnSkill = false;
        }
    }

    public void OnAttackL(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnAttack = true;
            DamageData dmgdata = new DamageData();
            dmgdata.damageAmount = Atk;
            bossEnemy.TakeDamage(dmgdata);
        }
        else if( context.canceled)
        {
            OnAttack = false;
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            TryInteract();
        }
    }

    public void InstanciateSkillEffect(GameObject go, Vector3 pos, Quaternion rotation)
    {
        Debug.Log("InsatntiateSkill");
        Instantiate(go, pos, rotation);
    }

#if UNITY_EDITOR
    public void DrawRunningStatusGUI()
    {

    }

    public SerializedObject GetSerializedBaseStatus()
    {
        if (player == null)
        {
            return null;
        }

        if (sPlayerStatus == null || sPlayerStatus.targetObject != player)
        {
            sPlayerStatus = new SerializedObject(player);
        }
        return sPlayerStatus;
    }
#endif
    public void AddSkill(SkillStatus data)
    {
        mySkills.Add(data);
        Debug.Log(data.name + "を入手");
    }
    public void UseSkill(int index)
    {
        SkillStatus status = mySkills[index];
        IUseSkill skill = null;
        if(index < mySkills.Count)
        {
            switch (status.skillId)
            {
                case SKILL.None: return;
                case SKILL.AOE:
                    skill = new AOE();
                    break;
                case SKILL.Blink:
                    skill = new blink();
                    break;
                case SKILL.AoE:
                    skill = new playerfollow();
                    break;
                case SKILL.Jump:
                    skill = new JumpATTACK();
                    break;
                case SKILL.Bomb:
                    skill = new skillbomb();
                    break;
            }
        }
        skill.Setup(status);
        skills[index] = skill;
        Debug.Log("statau" + status);
    }
    public void TryInteract()
    {
        //インタラクト可能な物を探す
        Collider[] hitColls = Physics.OverlapSphere(transform.position, InteractRange, InteractLayer);
        //チェックする
        foreach (var hitCollider in hitColls)
        {
            //そのオブジェクトがIInteractableを継承しているか確認
            if(hitCollider.TryGetComponent<IInteractable>(out var interactable))
            {
                //実行
                interactable.OnInteract(this);
                return;
            }
        }
    }

    public int TakeDamage(DamageData damageData)
    {
        //ダメージを受け取って現在のHPを減らす
       HP-=(int)damageData.damageAmount;
        return (int) damageData.damageAmount;
    }
    private void OnEnable()
    {
        //プレイヤーのトランスフォームを登録
        playerAnchor.Value = this.transform;
    }
    private void OnDisable()
    {
        playerAnchor = null;
    }
}
