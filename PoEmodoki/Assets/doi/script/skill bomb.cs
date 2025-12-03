using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
public class skillbomb : BaseSkill, IStatusView
{
    [SerializeField] GameObject bomb;
    [SerializeField] SkillStatus data;
    float ct;
    private SerializedObject sSkill;
    Image Icon;// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ct = -1;
        Icon = data.Icon;
    }

    // Update is called once per frame
    void Update()
    {
        /*if(ct<0)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Instantiate(bomb, new Vector3(transform.position.x, (transform.position.y-0.48f), transform.position.z), Quaternion.Euler(-90,0,0));
                ct = data.ct;
            }
        }
        if(ct >=0)
        {
            --ct;
        }*/
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
    protected override void UseSkill(GameObject obj)
    {
        if (ct < 0)
        {
                Instantiate(bomb, new Vector3(transform.position.x, (transform.position.y - 0.48f), transform.position.z), Quaternion.Euler(-90, 0, 0));
            ct = data.ct;
        }
        if (ct >= 0)
        {
            --ct;
        }
    }
}
