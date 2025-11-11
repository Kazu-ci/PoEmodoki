using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using UnityEngine.AI;
[Serializable]
public struct DamageData
{
    public float damageAmount;
    public DamageData(float damage)
    {
        damageAmount = damage;
    }
}

public class Enemy : MonoBehaviour
{
    //各ステータス
    [Header("ステータス")]
     protected float MaxHP;//最大HP
     protected float Strength;//攻撃力
    protected float AttackSpeed;//攻撃速度
     protected float AttackRange;//攻撃射程
     protected float AttackRate;//攻撃感覚
    protected float MoveSpeed;//移動速度
    [SerializeField] protected Texture[] textures;//テクスチャ
    protected float fov;//視野角
    [SerializeField] protected GameObject thisobj;//テクスチャ変更用
    [SerializeField] protected GameObject playerpos;
    protected float currentHP;//現在のHP
    protected NavMeshAgent navMeshAgent;
    protected float Distance;//プレイヤーとの距離
    protected GameObject weapon;//ドロップアイテム
    protected Animator animator;//アニメーション
    protected bool animationEnd;//アニメーション終了用フラグ
    private bool _isDead; // 重複死亡防止フラグ


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void OnAttackSet() { }
    public virtual void OnAttackEnd() { }
    public virtual void OnSumon() { }
    public virtual void Update()
    {
       // playerpos = EventBus.PlayerEvents.GetPlayerObject?.Invoke();
    }

    public float Getdistance()
    {
        //プレイヤーとの距離をさんしょう
        Vector3 offset=playerpos.transform.position-transform.position;
        return offset.magnitude;
    }

    
    public virtual int TakeDamage(DamageData dmg)
    {
        currentHP -= (int)dmg.damageAmount;
        return (int)dmg.damageAmount;
    }
    public virtual void OnDead()
    {
        if (_isDead) return;
        _isDead = true;
        Destroy(gameObject);
    }

    public float Getdaamge()
    {
        return Strength;
    }
    public bool AnimationEnd(string stateName)
    {
        // 現在のステート情報を取得
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // ステート名をハッシュ化して比較
        int stateHash = Animator.StringToHash("Base Layer." + stateName);

        // 該当ステートでかつ normalizedTime >=1 なら終了とみなす
        if (stateInfo.fullPathHash == stateHash && stateInfo.normalizedTime >= 1f)
        {
            return true;
        }

        return false;
    }
    public static bool Probability(float fPersent)//確立判定用メソッド
    {
        float fProbabilityRate = UnityEngine.Random.value * 100;
        if (fPersent == 100f && fProbabilityRate == fPersent)
        {
            return true;
        }
        else if (fPersent > fProbabilityRate)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
