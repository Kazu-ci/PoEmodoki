#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
public class blink : BaseSkill
{
    float speed;
    bool On=true;
    bool used = false;

    void Blink(PlayerCon con)
    {
        float h = Input.GetAxisRaw("Horizontal"); 

        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = new Vector3(h, 0, v).normalized;
        Quaternion characterRotation = con.transform.rotation;
        Vector3 worldMoveDirection = characterRotation * inputDirection;
        Vector3 moveVector = worldMoveDirection* speed;
        // TODO: à⁄ìÆÇ≥ÇπÇÈï˚ñ@ÇçlÇ¶Çƒ.
        //pc.Move(moveVector);
        On = false;
    }
    public override void Setup(SkillStatus status)
    {
    }

    public override void UseSkill(PlayerCon con)
    {
        if ( On == true)
        {
            Blink(con);
            used = true;
        }
        else
        {
            used = false;
        }
        /*if (used == true)
        {
            ct = data.ct;
        }
        else
        {
            --ct;
        }*/
    }
}
