using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Soul : MonoBehaviour, IInteractable, IStatusView
{
    [SerializeField] SkillStatus data;
    Image Icon;
    float Ct;
#if UNITY_EDITOR
    private SerializedObject sSkill;
#endif

    void Start()
    {
        Icon = data.Icon;
        Ct = data.ct;
    }

    public void OnInteract(PlayerCon player)
    {
        if (data != null)
        {
            player.AddallSkill(data);
            Debug.Log(data + "“üŽè");
            Destroy(gameObject);
        }
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
#if UNITY_EDITOR
    public void DrawRunningStatusGUI()
    {

    }
#endif
}
