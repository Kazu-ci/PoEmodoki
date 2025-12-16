using UnityEngine;

public class TalkState : StateMachine<GameCon>.StateBase
{
    public override void OnStart()
    {
        Time.timeScale = 0f;
    }

    public override void OnUpdate()
    {
        // 会話中の特殊な監視ロジック（例：Fungus完了待ちなど）

        // Fungusが終了したら ChangeState(State.Combat) を呼び出す必要がある。
        // (例: Flowchart.ExecuteBlock("EndTalk") の最後に Call Method で Combat に戻す)
    }

    public override void OnEnd()
    {
        // TalkState終了時のクリーンアップ
    }
}
