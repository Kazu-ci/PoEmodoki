using UnityEngine;


[CreateAssetMenu(menuName = "Game/Player Anchor")]
public class PlayerAnchor : ScriptableObject
{
    //‚±‚±‚ÉƒvƒŒƒCƒ„[‚ÌTransform‚ª“ü‚é
    //“G‚Í‚±‚±‚ğŒ©‚É—ˆ‚é‚¾‚¯‚Å‚¢‚¢
    public Transform Value;

    public void OnDisable()
    {
        Value = null;
    }
}