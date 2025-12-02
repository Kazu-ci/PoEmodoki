using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
public class Tossin : MonoBehaviour
{
    [SerializeField] SkillStatus skill;
    [SerializeField] CharacterController CC;
    float speed;
    public KeyCode SpawnKey = KeyCode.F;
    public float dashDuration = 0.2f;
    float dashTimer;
    bool ISDASH = false;
    float h, v;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = skill.speed;
        CC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(SpawnKey))
        {
            isDash();
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }
        if(ISDASH)
        {
            dashTimer -= Time.deltaTime;
            Vector3 inputDirection = new Vector3(h, 0, v).normalized;
            Quaternion characterRotation = CC.transform.rotation;
            Vector3 worldMoveDirection = characterRotation * inputDirection;
            Vector3 moveVector = worldMoveDirection * speed;
            CC.Move(moveVector * Time.deltaTime);
            if(dashTimer<0)
            {
                ISDASH = false;
            }
        }
    }
    void isDash()
    {
        dashTimer = dashDuration;
        ISDASH = true;
    }
}

