#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class AOE : BaseSkill,IStatusView
{
    [SerializeField] SkillStatus data;
    [SerializeField] GameObject Donut;
    [SerializeField] GameObject player;
    public KeyCode spawnKey = KeyCode.V;
    public float Distance;
    Vector3 forwardDirection;
    Vector3 offset;
    Vector3 point;
    Image Icon;
    float Ct;
    float h, v;
#if UNITY_EDITOR
    private SerializedObject sSkill;
#endif
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Icon = data.Icon;
        Ct = data.ct;
    }

    // Update is called once per frame
    void Update()
    {
       //forwardDirection = player.transform.forward;
       //offset = forwardDirection * Distance;
       // point = player.transform.position + offset;
       // point.y = 0;
       // if (Input.GetKeyDown(spawnKey))
       // {
       //     Instantiate(Donut, point, Quaternion.Euler(-90, 0, 0));
       // }

    }
    public void DrawRunningStatusGUI()
    {

    }
#if UNITY_EDITOR
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
    protected override void UseSkill(GameObject obj)
    {
        forwardDirection = obj.transform.forward;
        offset = forwardDirection * Distance;
        point = obj.transform.position /*+ offset*/;
        point.y = 0;
        Instantiate(Donut, point, Quaternion.Euler(-90, 0, 0));
    }

}
 