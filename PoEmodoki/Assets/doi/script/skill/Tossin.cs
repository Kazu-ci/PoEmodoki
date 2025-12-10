using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
public class Tossin : BaseSkill
{
    float speed;
    public KeyCode SpawnKey = KeyCode.F;
    public float dashDuration = 0.2f;
    float dashTimer;
    float Ct;
    float Count;
    bool ISDASH = false;
    float h, v;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Setup(SkillStatus status)
    {
        speed = status.speed;
        Ct = status.ct;
    }

    public override void UseSkill(PlayerCon con)
    {
            isDash();
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        if (ISDASH&&Count>Ct)
        {
            dashTimer -= Time.deltaTime;
            Vector3 inputDirection = new Vector3(h, 0, v).normalized;
            Quaternion characterRotation = con.transform.rotation;
            Vector3 worldMoveDirection = characterRotation * inputDirection;
            Vector3 moveVector = worldMoveDirection * speed;
            // TODO: ˆÚ“®‚³‚¹‚é•û–@‚ğl‚¦‚Ä.
            //CC.Move(moveVector * Time.deltaTime);
            if (dashTimer < 0)
            {
                ISDASH = false;
            }
            Count = 0;
        }
        if(Count<=Ct)
        {
            ++Count;
        }
    }
    void isDash()
    {
        dashTimer = dashDuration;
        ISDASH = true;
    }

}

