using UnityEngine;

public class EndState : StateMachine<GameCon>.StateBase
{
    public override void OnStart()
    {
        Owner.Flowchart.SendFungusMessage("PlayerDead");
        Owner.ChangeTalk();
    }

}
