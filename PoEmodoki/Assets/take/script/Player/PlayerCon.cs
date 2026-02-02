using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(CharacterController))]
public class PlayerCon : MonoBehaviour,IStatusView
{
    //スクリプタブルオブジェクト
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

    //CharactorController関連
    [SerializeField]private CharacterController charaCon;
    private Vector2 moveInput = default;

    //待機命令
    private WaitForSeconds wait;

    //無敵時間
    [SerializeField] private float invincibleDuration = 1.0f;
    //無敵フラグ
    private bool isInvincible = false;

    StateMachine<PlayerCon> stateMachine;
    enum state
    {
        Idol,
        Move,
        Attack,
        Hit,
        SkillAttack,
        Dead,
    }

    private void Awake()
    {
        MaxHP = player.PlayerHp;
        HP = MaxHP;
        MoveSpeed = player.PlayerSpeed;
        AtkSpeed = player.PlayerAtkSpeed;
        CastSpeed = player.PlayerCastSpeed;
        Length = player.PlayerLength;
        ElementDefense = player.PlayerElementDefense;
        Critical = player.PlayerCritical;
        Atk = player.PlayerAtk;

        move = PlayerInput.actions["Move"];

        wait = new WaitForSeconds(0.1f);

        stateMachine = new StateMachine<PlayerCon>(this);
        stateMachine.Add<MoveState>((int)state.Move);
        stateMachine.Add<IdolState>((int)state.Idol);
        stateMachine.Add<AttackState>((int)state.Attack);
        stateMachine.Add<HitState>((int)state.Hit);
        stateMachine.Add<SkillAttackState>((int)state.SkillAttack);
        stateMachine.Add<DeadState>((int)state.Dead);
        stateMachine.Onstart((int)state.Idol);
        
    }
    void Update()
    {

        if(GameCon.Instance != null && GameCon.Instance.currentState == GameCon.GameState.Talk)
        {
            moveInput = Vector2.zero;
            if(!(stateMachine.CurrentState is IdolState))
            {
                stateMachine.ChangeState((int)state.Idol);
            }
            return;
        }

        moveInput = move.ReadValue<Vector2>();

        stateMachine.OnUpdate();
    }

    public void MoveCharacter(Vector3 motion)
    {
        charaCon.Move(motion);
    }

    public void Getvalue(float s)
    {
        MoveSpeed = s;
    }
    //------------------------------------------------------------------------------------------------
    //以下ステート管理
    //------------------------------------------------------------------------------------------------
    public class MoveState : StateMachine<PlayerCon>.StateBase
    {
        public override void OnStart()
        {
            Debug.Log("Move");
            Owner.anim.CrossFade(Owner.AnimRun, 0.1f);
        }
        public override void OnUpdate()
        {
            //移動処理
            Vector3 direction = new Vector3(Owner.moveInput.x,0, Owner.moveInput.y).normalized;
            
            if (direction.magnitude >= 0.1f)
            {
                //回転処理
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f,targetAngle,0f);
                Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, targetRotation, 10f * Time.deltaTime);

                //移動ベクトル
                Vector3 moveDir = targetRotation * Vector3.forward;

                //移動の実行
                Owner.MoveCharacter(moveDir * Owner.MoveSpeed * Time.deltaTime);
            }

            //以下ステート遷移判定
            if (Owner.OnAttack)
            {
                StateMachine.ChangeState((int)state.Attack);
            }
            else if (Owner.OnSkill)
            {
                StateMachine.ChangeState((int)state.SkillAttack);
            }
            else if (Owner.moveInput == Vector2.zero)
            {
                StateMachine.ChangeState((int)state.Idol);
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
            if(Owner.moveInput != Vector2.zero)
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
            Owner.OnAttack = false;
            if(SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(SoundManager.SE.Attack);
            }
        }
        public override void OnUpdate()
        {
            AnimatorStateInfo stateInfo = Owner.anim.GetCurrentAnimatorStateInfo(0);

            if(Owner.anim.IsInTransition(0))
            {
                return;
            }
            if(!stateInfo.IsName("Attack"))
            {
                return;
            }

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
            }
        }
        public override void OnEnd()
        {

        }
        private void hitCheckColl()
        {
            float dagger = 5f;
            

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
            Owner.anim.CrossFade(Owner.AnimTakingDamage, 0.1f);
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(SoundManager.SE.Hit);
            }
        }
        public override void OnUpdate()
        {
            AnimatorStateInfo stateInfo = Owner.anim.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("TakingDamage") && stateInfo.normalizedTime >= 1.0f)
            {
                StateMachine.ChangeState((int)(state.Idol));   
            }
            else
            {
                StateMachine.ChangeState((int)state.Dead);
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
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(SoundManager.SE.Dead);
            }
        }
        public override void OnUpdate()
        {

        }
        public override void OnEnd()
        {

        }
    }
    //------------------------------------------------------------------------------------------------
    //以下プレイヤーイベント
    //------------------------------------------------------------------------------------------------
    public void MovePlayer(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
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
    //------------------------------------------------------------------------------------------------
    //以下その他のメソッド
    //------------------------------------------------------------------------------------------------
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
                GameCon.Instance.TryExecuteInteraction();
                return;
            }
        }
    }

    public int TakeDamage(DamageData damageData)
    {
        if(isInvincible)
        {
            return 0;
        }
        //ダメージを受け取って現在のHPを減らす
       HP-=(int)damageData.damageAmount;
        if(HP > 0)
        {
            stateMachine.ChangeState((int)state.Hit);

            StartCoroutine(OnInvincibleRoutine());
        }
            return (int)damageData.damageAmount;
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
    private IEnumerator OnInvincibleRoutine()
    {
        Debug.Log("無敵");
        isInvincible = true;

        yield return new WaitForSeconds(invincibleDuration);

        isInvincible = false;

    }
}
