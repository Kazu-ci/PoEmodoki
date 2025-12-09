using Fungus;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorSelectImage : MonoBehaviour
{
    public Image cursorImage;         // カーソル用のImage
    public Image[] targetImages; // 判定対象のImageたち
    public Image[] targetslots;
    [SerializeField] Slots slotData;
    private Sprite selectedSprite;     // 保存した Sprite
    private bool hasSelected = false;
    void Start()
    {
        // ★ Scene 読み込み時に ScriptableObject から UI へ復元
        if (slotData.slot1 != null) targetslots[0].sprite = slotData.slot1.sprite;
        if (slotData.slot2 != null) targetslots[1].sprite = slotData.slot2.sprite;
        if (slotData.slot3 != null) targetslots[2].sprite = slotData.slot3.sprite;
    }
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Return))
        {

       

            foreach (var img in targetImages)
            {
                if (IsRectOverlapping(cursorImage.rectTransform, img.rectTransform))
                {
                    selectedSprite = img.sprite;
                    hasSelected = true;
                    Debug.Log("画像を選択しました: " + img.name);
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
                            slotData.slot1.sprite = selectedSprite;
                        if (slot.name == "slot2" )
                            slotData.slot2.sprite = selectedSprite;
                        if (slot.name == "slot3" )
                            slotData.slot3.sprite = selectedSprite;
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
