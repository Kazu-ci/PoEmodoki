#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.Android;

public class TalkState : StateMachine<GameCon>.StateBase
{
    public override void OnStart()
    {
        Debug.Log("Talk");
        //Time.timeScale = 0f;
    }

    public override void OnUpdate()
    {
        // 会話中の特殊な監視ロジック（例：Fungus完了待ちなど）

        // (例: Flowchart.ExecuteBlock("EndTalk") の最後に Call Method で Combat に戻す)
    }
    public override void OnEnd()
    {
        // TalkState終了時のクリーンアップ
    }
}
