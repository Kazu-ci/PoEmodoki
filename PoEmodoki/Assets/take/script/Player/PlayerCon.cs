using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;




public class PlayerCon : MonoBehaviour,IStatusView
{
    [SerializeField]PlayerStatus player;
    private SerializedObject sPlayerStatus;
    [SerializeField]PlayerInput PlayerInput;
    [SerializeField]PlayerAnchor playerAnchor;
    [SerializeField]SkillStatus skill;//仮
    InputAction move;
    InputAction skill1,skill2,skill3,skill4;

    public List<SkillStatus> mySkills = new List<SkillStatus>();
    private Vector2 moveVec = default;
    bool OnSkill = false;

    int MaxHP;
    int Defense;
    int MP;
    float MoveSpeed;
    float AtkSpeed;
    float CastSpeed;
    float Length;
    float ElementDefense;
    float Critical;
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
        Defense = player.PlayerDefense;
        MP = player.PlayerMp;
        MoveSpeed = player.PlayerSpeed;
        AtkSpeed = player.PlayerAtkSpeed;
        CastSpeed = player.PlayerCastSpeed;
        Length = player.PlayerLength;
        ElementDefense = player.PlayerElementDefense;
        Critical = player.PlayerCritical;

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
        stateMachine.Add<PoseState>((int)state.pose);
        stateMachine.Onstart((int)state.Idol);
        //デバッグ用スキル
        mySkills.Add(skill);
    }
    void Update()
    {
        stateMachine.OnUpdate();
    }

    public class MoveState : StateMachine<PlayerCon>.StateBase
    {
        public override void OnStart()
        {
            Debug.Log("Move");
        }
        public override void OnUpdate()
        {
            Owner.gameObject.transform.Translate
                (new Vector3(Owner.moveVec.x, 0, Owner.moveVec.y) * Owner.MoveSpeed);

            if (Owner.move.ReadValue<Vector2>() == new Vector2(0, 0))
            {
                StateMachine.ChangeState((int)state.Idol);
            }
            if(Owner.skill1.IsPressed())
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
        }
        public override void OnUpdate()
        {
            if(Owner.moveVec != Vector2.zero) 
            {
                StateMachine.ChangeState((int)state.Move);
            }
            if(Owner.OnSkill)
            {
                StateMachine.ChangeState((int)state.SkillAttack);
            }
        }
        public override void OnEnd()
        {

        }
    }

    public class AttackState : StateMachine<PlayerCon>.StateBase
    {
        public override void OnStart()
        {
            
        }
        public override void OnUpdate()
        {

        }
        public override void OnEnd()
        {

        }
    }

    public class HitState : StateMachine<PlayerCon>.StateBase
    {
        public override void OnStart()
        {

        }
        public override void OnUpdate()
        {

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
        }
        public override void OnUpdate()
        {
            if (Owner.OnSkill == false)//デバッグ用スキル
            {
                StateMachine.ChangeState((int)(state.Idol));
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
        }
        public override void OnUpdate()
        {
            //シーン遷移
            //アニメーション
        }
        public override void OnEnd()
        {

        }
    }

    public class PoseState : StateMachine<PlayerCon>.StateBase
    {
        public override void OnStart()
        {

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
            UseSkill(0);//デバッグ用スキル
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
            UseSkill(0);//デバッグ用スキル
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
            UseSkill(0);//デバッグ用スキル
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
            UseSkill(0);//デバッグ用スキル
        }
        else if (context.canceled)
        {
            OnSkill = false;
        }
    }

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

    public void AddSkill(SkillStatus data)
    {
        mySkills.Add(data);
        Debug.Log(data.name + "を入手");
    }
    public void UseSkill(int index)
    {
        SkillStatus skill = mySkills[index];
        if(index < mySkills.Count)
        {
            //ここでプレハブを生成
            if(skill.skillPre!= null)
            {
                //プレイヤーの前方に生成
                Vector3 spawnPos = transform.position + transform.forward * 1.5f;
                GameObject skillObj = Instantiate(skill.skillPre, spawnPos, transform.rotation);
            }
        }
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
