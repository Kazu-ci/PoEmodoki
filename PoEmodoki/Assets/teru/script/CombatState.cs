using UnityEngine;

public class CombatState : StateMachine<GameCon>.StateBase
{
    public override void OnStart()
    {
        Debug.Log("Combat");
        Time.timeScale = 1f;
    }

    public override void OnUpdate()
    {
        if (Owner.pStatus.PlayerHp <= 0) { 
            Owner.Flowchart.SendFungusMessage("PlayerDead");
            Owner.ChangeTalk();
        }
        if (Owner.eStatus.EnemyName == "Boss" && Owner.eStatus.EnemyHp <= 200)
        {
            Owner.TriggerNextConversation();
        }
        if (Owner.eStatus.EnemyName == "Boss" && Owner.eStatus.EnemyHp <= 0)
        {
            Owner.TriggerNextConversation();
        }
    }

    public override void OnEnd()
    {
        // CombatState終了時のクリーンアップ
    }
}
