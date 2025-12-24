using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

public class keepskill : MonoBehaviour
{
    [SerializeField] RectTransform slot1;
    [SerializeField] RectTransform slot2;
    [SerializeField] RectTransform slot3;
    [SerializeField] RectTransform ImageRect;
    [SerializeField] SkillStatus SkillData;
    [SerializeField] List<RectTransform> otherImages;
    public Transform slotParent;
    public GameObject UIslot;
     PlayerCon player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Refresh();
    }

    // UI更新
    public void Refresh()
    {
        // 既存スロットを削除
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
        otherImages.Clear();

        // PlayerのallSkillを元にUIスロット生成
        foreach (var skill in player.allskill)
        {
            GameObject slotObj = Instantiate(UIslot, slotParent);
            RectTransform slotRect = slotObj.GetComponent<RectTransform>();
            //SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();
            //slotUI.SetSkill(skill);

            otherImages.Add(slotRect);
        }
    }


// Update is called once per frame
void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            foreach(var image in otherImages)
            {
                if (IsOverlapping(ImageRect, image))
                {
                    Debug.Log($"{image.name} と重なっています！");
                }
            }
        }
    }
    public static bool IsOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Rect rectA = GetWorldRect(rect1);
        Rect rectB = GetWorldRect(rect2);

        return rectA.Overlaps(rectB);
    }
    public static Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        Vector2 size = new Vector2(
            Vector3.Distance(corners[0], corners[3]),   
            Vector3.Distance(corners[0], corners[1])   
        );

        return new Rect(corners[0], size);
    }
}
