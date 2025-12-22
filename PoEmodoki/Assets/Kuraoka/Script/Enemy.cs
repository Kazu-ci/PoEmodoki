using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public struct DamageData
{
    public float damageAmount;   // ダメージ量を格納する変数

    public DamageData(float damage)
    {
        damageAmount = damage;   // 値をセット
    }
}

public class Enemy : MonoBehaviour
{
    private GameCon gameCon;
    [Header("名前")]
    protected string name;                 // 敵の名前

    // 各ステータス
    [Header("ステータス")]
    protected float MaxHP;                 // 最大HP
    protected float Strength;              // 攻撃力
    protected float AttackSpeed;           // 攻撃速度
    protected float AttackRange;           // 攻撃射程
    protected float AttackRate;            // 攻撃間隔
    protected float MoveSpeed;             // 移動速度
    protected float lookPlayerDir;
    protected BaseSkill currentSkill;
    [SerializeField] protected PlayerAnchor playerAnchor;
    [SerializeField] protected Texture[] textures;  // テクスチャ（見た目変更用）

    protected float fov;                   // 視野角（プレイヤーを見つける範囲）

    [SerializeField] protected GameObject thisobj;  // テクスチャ変更用オブジェクト

    [SerializeField] protected  GameObject player; // プレイヤーのオブジェクトを保持
    [SerializeField] protected   List<SkillStatus> skills;
    public NavMeshAgent Agent => navMeshAgent;
    public GameObject Player => player;
    protected Vector3 playerpos;

    protected float currentHP;             // 現在のHP

    protected NavMeshAgent navMeshAgent;   // NavMeshAgent（移動AI）

    protected float Distance;              // プレイヤーとの距離

    protected GameObject weapon;           // ドロップアイテム

    protected Animator animator;           // アニメーション制御用

    protected bool animationEnd;           // アニメーション終了フラグ

    private bool _isDead;                  // 死亡済みフラグ（二重で死なないように）

    // 攻撃アニメーション開始時に呼ばれる（子クラスで上書き用）
    public virtual void OnAttackSet() { }

    // 攻撃アニメーション終了時に呼ばれる（子クラスで上書き用）
    public virtual void OnAttackEnd() { }

    // 召喚アニメーション時…などに使う用（子クラスで上書き）
    public virtual void OnSumon() { }

    protected virtual void Update()
    {
        if (gameCon == null)
        {
            gameCon = GameCon.Instance;
            if (gameCon == null) return;
        }
        if (gameCon.currentState == GameCon.GameState.Talk)
        {
            return;
        }
        playerpos = playerAnchor.Value.position;
    }

    public float Getdistance()
    {
        // プレイヤーとの距離を Vector3 で計算して返す
        Vector3 offset = player.transform.position - transform.position;
        return offset.magnitude; // ベクトルの長さ = 距離
    }

    // NavMesh 内のランダムな地点を取得するメソッド
    public Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 50; i++)
        {
            // 半径内でランダムな位置を生成
            Vector3 rand = center + new Vector3(
                UnityEngine.Random.Range(-radius, radius),
                0,
                UnityEngine.Random.Range(-radius, radius)
            );

            rand.y = center.y; // 高さを一定にする

            NavMeshHit hit;

            // 生成したランダム点を NavMesh 上にスナップ
            if (NavMesh.SamplePosition(rand, out hit, radius, NavMesh.AllAreas))
                return hit.position;
        }

        // 見つからない場合は中心を返す
        return center;
    }

    public virtual int TakeDamage(DamageData dmg)
    {
        // ダメージを受け取り、現在HPを減らす
        currentHP -= (int)dmg.damageAmount;
        return (int)dmg.damageAmount; // 実際に受けたダメージ量を返す
    }

    public virtual void OnDead()
    {
        if (_isDead) return; // すでに死んでいたら処理しない

        _isDead = true;      // 死亡フラグを立てる
        Destroy(gameObject); // 敵のオブジェクトを破壊
    }

    public float Getdaamge()
    {
        return Strength;     // 敵の攻撃力を返す
    }

    public void UseSkill(int index)
    {
        if (index < 0 || index >= skills.Count) return;

        SkillStatus status = skills[index];
        BaseSkill skill = CreateSkillInstance(status.skillId);

        if (skill == null) return;

        skill.Setup(status);
        skill.EnemyUseSkill(this, status);
       currentSkill = skill;
    }
    public void InstanciateSkillEffect(GameObject go, Vector3 pos, Quaternion rotation)
    {
        Debug.Log("InsatntiateSkill");
        Instantiate(go, pos, rotation);
    }
    protected BaseSkill CreateSkillInstance(SKILL id)
    {
        switch (id)
        {
            case SKILL.AOE:
                return new AOE();
            case SKILL.Blink:
                return new blink();
            case SKILL.Tossin:
                return new Tossin();
            case SKILL.Bomb:
                return new skillbomb();
            default:
                return null;
        }
    }
    public bool AnimationEnd(string stateName)
    {
        // 現在のアニメーション状態を取得
       // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // アニメーションステート名をハッシュに変換
        int stateHash = Animator.StringToHash("Base Layer." + stateName);

        // 該当ステートで、かつ アニメーションが最後まで再生されたら true を返す
        //if (stateInfo.fullPathHash == stateHash && stateInfo.normalizedTime >= 1f)
        //{
            return true;
        //}

        //return false;
    }

    // 確率判定用メソッド（%で判定する）
    public static bool Probability(float fPersent)
    {
        // 0?100 のランダム値
        float fProbabilityRate = UnityEngine.Random.value * 100;

        // 100% のときだけ例外処理をしてる（仕様に合わせて残してる）
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
