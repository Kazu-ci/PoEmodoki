using UnityEngine;
using UnityEngine.Rendering;
public class Enemy : MonoBehaviour
{
    //各ステータス
    [Header("ステータス")]
    [SerializeField] protected float MaxHP;//最大HP
    [SerializeField] protected float Strength;//攻撃力
    [SerializeField] protected float AttackSpeed;//攻撃速度
    [SerializeField] protected float AttackRange;//攻撃射程
    [SerializeField ] protected float AttackRate;//攻撃感覚
    [SerializeField] protected Texture[] textures;//テクスチャ
    [SerializeField] protected float fov;//視野角
    [SerializeField] protected GameObject thisobj;//テクスチャ変更用
    protected float currentHP;//現在のHP
    protected float Distance;//プレイヤーとの距離
    protected GameObject weapon;//ドロップアイテム
    protected Animator animator;//アニメーション
    protected bool AnimationEnd;//アニメーション終了用フラグ
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
}
