using UnityEngine;

public class test : MonoBehaviour
{

    public static test Instance {  get; private set; }
    [Header("敵のステータス")]
    //敵のステータス
    public int EnemyHp;                 //敵のHP
    public int EnemyAtk;                //敵の攻撃力
    public int EnemyDefense;            //敵の防御力
    public float EnemySpeed;            //敵の移動速度
    public float EnemyAtkSpeed;         //敵の攻撃速度
    public float EnemyCastSpeed;        //敵の詠唱速度
    public float EnemyLength;           //敵の射程
    public float EnemyElementDefense;   //敵の属性耐性
    [Header("プレイヤーのステータス")]
    //プレイヤーのステータス
    public int PlayerHp;                //プレイヤーのHP
    public int PlayerDefense;           //プレイヤーの防御力
    public float PlayerSpeed;           //プレイヤーの移動速度
    public int PlayerMp;                //プレイヤーのMP
    public float PlayerAtkSpeed;        //プレイヤーの攻撃速度
    public float PlayerCastSpeed;       //プレイヤーの詠唱速度
    public float PlayerLength;          //プレイヤーの射程
    public float PlayerElementDefense;  //プレイヤーの属性耐性
    public float PlayerCritical;        //クリティカル
    [Header("スキルのステータス")]
    //スキルのステータス
    public float SkillAtk;              //スキルの攻撃力
    public float SkillSpeed;            //スキルの飛んでく速度
    public float SkillCoolTime;         //スキルのクールタイム
    public int SkillElementDmg;         //スキルの属性攻撃力
    [Header("武器のステータス")]
    //武器のステータス
    public int WeponAtk;                //武器の攻撃力
    public int WeponElementDmg;         //武器の武器の属性ダメージ
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EnemyHp = 
    }

}
