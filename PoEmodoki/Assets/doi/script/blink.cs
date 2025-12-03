#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
public class blink : MonoBehaviour,IStatusView
{
    [SerializeField] SkillStatus data;
#if UNITY_EDITOR
    private SerializedObject sSkill;
#endif
    [SerializeField] CharacterController pc;
    float speed;
    float ct;
    Image Icon;
    bool On=true;
    bool used = false;
    float count = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = data.speed;
        ct = data.ct;
        Icon = data.Icon;
        Icon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
       if (ct <count) {
            On = true;
        }
        if (Input.GetKeyDown(KeyCode.E)&&On == true)
        {
            Blink();
            used = true;
        }
        else
        {
            used = false;
        }
        if(used == true)
        {
            ct = data.ct;
        }
        else
        {
            --ct;
        }
        
    }
    void Blink()
    {
        float h = Input.GetAxisRaw("Horizontal"); 

        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = new Vector3(h, 0, v).normalized;
        Quaternion characterRotation = pc.transform.rotation;
        Vector3 worldMoveDirection = characterRotation * inputDirection;
        Vector3 moveVector = worldMoveDirection* speed;
        pc.Move(moveVector);
        On = false;
    }
#if UNITY_EDITOR
    public void DrawRunningStatusGUI()
    {
        EditorGUILayout.FloatField("Œ»Ý‚ÌƒuƒŠƒ“ƒN‘¬“x:", speed);
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
#endif
}
