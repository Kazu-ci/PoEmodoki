using UnityEngine;

public class Enemy : MonoBehaviour
{
    //各ステータス
    [Header("ステータス")]

    [SerializeField] protected float MaxHP;//最大HP
    [SerializeField] protected float Strength;//攻撃力
    [SerializeField] protected float AttackSpeed;//攻撃速度
    [SerializeField] protected float AttackRange;//攻撃射程
    protected float currentHP;//現在のHP
    protected float Distance;//プレイヤーとの距離

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
