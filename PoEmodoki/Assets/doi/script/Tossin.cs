using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
public class Tossin : BaseSkill,IStatusView
{
    [SerializeField] SkillStatus data;
    [SerializeField] GameObject obj;
    CharacterController CC;
    float speed;
    public KeyCode SpawnKey = KeyCode.F;
    public float dashDuration = 0.2f;
    float dashTimer;
    float Ct;
    float Count;
    bool ISDASH = false;
    float h, v;
    private SerializedObject sSkill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = data.speed;
        Ct = data.ct;
        CC = obj.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
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
        */
    }
    public void DrawRunningStatusGUI()
    {

    }

    public SerializedObject GetSerializedBaseStatus()
    {
        if (data == null)
        {
            return null;
        }

        if (sSkill == null || sSkill.targetObject != data)
        {
            sSkill = new SerializedObject(data);
        }
        return sSkill;
    }
    void isDash()
    {
        dashTimer = dashDuration;
        ISDASH = true;
    }
    protected override void UseSkill(GameObject obj)
    {
            isDash();
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        if (ISDASH&&Count>Ct)
        {
            dashTimer -= Time.deltaTime;
            Vector3 inputDirection = new Vector3(h, 0, v).normalized;
            Quaternion characterRotation = CC.transform.rotation;
            Vector3 worldMoveDirection = characterRotation * inputDirection;
            Vector3 moveVector = worldMoveDirection * speed;
            CC.Move(moveVector * Time.deltaTime);
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
}

