//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Android;

public class TalkState : StateMachine<GameCon>.StateBase
{
    public override void OnStart()
    {
        Debug.Log("Talk");
    }

}
