using UnityEngine;
using UnityEngine.UI;

public class CursorSelectImage : MonoBehaviour
{
    public Image cursorImage;         // カーソル用のImage
    public Image[] targetImages;      // 判定対象のImageたち
    public Image[] targetslots;
   
    private Sprite selectedSprite;     // 保存した Sprite
    private bool hasSelected = false;
    
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Return))
        {

            Image selectedImage = null;
            Image sloted = null;
            Sprite s = null;

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
                        hasSelected = false; // 使ったらリセット
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
