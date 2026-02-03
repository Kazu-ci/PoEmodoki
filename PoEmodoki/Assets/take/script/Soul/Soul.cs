#if UNITY_EDITOR
using UnityEditor;
#endif
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
            GameCon.Instance.rei = false;
            player.AddallSkill(data);
            Debug.Log(data + "“üŽè");
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(SoundManager.SE.GetSoul);
            }
            Destroy(gameObject);
        }
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
#if UNITY_EDITOR
    public void DrawRunningStatusGUI()
    {

    }
#endif
}
