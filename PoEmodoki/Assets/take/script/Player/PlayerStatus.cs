using UnityEngine;

[CreateAssetMenu(fileName = "new PlayerStatus" , menuName = "Player/Status")]
public class PlayerStatus : ScriptableObject
{
    [Header("プレイヤーのステータス")]
    public int PlayerHp;                //プレイヤーのHP
    public int PlayerDefense;           //プレイヤーの防御力
    public float PlayerSpeed;           //プレイヤーの移動速度
    public int PlayerMp;                //プレイヤーのMP
    public float PlayerAtkSpeed;        //プレイヤーの攻撃速度
    public float PlayerCastSpeed;       //プレイヤーの詠唱速度
    public float PlayerLength;          //プレイヤーの射程
    public float PlayerElementDefense;  //プレイヤーの属性耐性
    public float PlayerCritical;        //クリティカル
}
