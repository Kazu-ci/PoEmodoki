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
        // 戦闘中の GameCon の監視ロジック（例：プレイヤーHPゼロ判定など）
    }

    public override void OnEnd()
    {
        // CombatState終了時のクリーンアップ
    }
}
