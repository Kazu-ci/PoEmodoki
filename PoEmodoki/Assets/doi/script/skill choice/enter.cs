using Fungus;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CursorSelectImage : MonoBehaviour
{
    public Image cursorImage;         // カーソル用のImage
    public Image[] targetImages; // 判定対象のImageたち
    public Image[] targetslots;
    [SerializeField] SkillStatus slot1;
    [SerializeField] SkillStatus slot2;
    [SerializeField] SkillStatus slot3;
    private Sprite selectedSprite;
    private string selectedname;
    private bool hasSelected = false;
    void Start()
    {
        // ★ Scene 読み込み時に ScriptableObject から UI へ復元
        if (slot1 != null) targetslots[0].sprite = slot1.sprite;
        if (slot2 != null) targetslots[1].sprite = slot2.sprite;
        if (slot3 != null) targetslots[2].sprite = slot3.sprite;
        if (slot1 != null) targetslots[0].sprite = null;
        if (slot2 != null) targetslots[1].sprite = null;
        if (slot3 != null) targetslots[2].sprite = null;
    }
    void Update()
    {
        Debug.Log(slot1.Skillname);
        if (Input.GetKeyDown(KeyCode.Return))
        {

       

            foreach (var img in targetImages)
            {
                if (IsRectOverlapping(cursorImage.rectTransform, img.rectTransform))
                {
                    selectedSprite = img.sprite;
                    selectedname = img.transform.parent.name;
                    hasSelected = true;
                    Debug.Log("画像を選択しました: " + img.name);
                    Debug.Log(slot1.Skillname);
                    break; 
                }
            }
            if (hasSelected)
            {
                foreach (var slot in targetslots)
                {
                    if (IsRectOverlapping(cursorImage.rectTransform, slot.rectTransform))
                    {
                        slot.sprite = selectedSprite;
                        Debug.Log("スロットに画像をセットしました: " + slot.name);
                        if (slot.name == "slot1" )
                        {
                            slot1.sprite = selectedSprite;
                            slot1.Skillname = selectedname;
                            Debug.Log(slot1.Skillname);
                        }
                       
                        else if (slot.name == "slot2" )
                        {
                            slot2.sprite = selectedSprite;
                            slot2.Skillname = selectedname;
                            Debug.Log(slot2.Skillname);
                        }
                        else if (slot.name == "slot3" )
                        {
                            slot3.sprite = selectedSprite;
                            slot3.Skillname = selectedname;
                        }
                        hasSelected = false; 
                        break;
                    }
                }
            }
            
        }
    }

    // RectTransform同士の重なり判定
    bool IsRectOverlapping(RectTransform rectA, RectTransform rectB)
    {
        Rect a = GetWorldRect(rectA);
        Rect b = GetWorldRect(rectB);
        return a.Overlaps(b);
    }

    // RectTransformからワールド座標のRectを取得
    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];
        return new Rect(bottomLeft.x, bottomLeft.y,
                        topRight.x - bottomLeft.x,
                        topRight.y - bottomLeft.y);
    }
}
