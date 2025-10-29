using UnityEngine;
using UnityEngine.UI;

public class CursorSelectImage : MonoBehaviour
{
    public Image cursorImage;         // カーソル用のImage
    public Image[] targetImages;      // 判定対象のImageたち

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Image selectedImage = null;

            foreach (var img in targetImages)
            {
                if (IsRectOverlapping(cursorImage.rectTransform, img.rectTransform))
                {
                    selectedImage = img;
                    break; // 最初に重なったImageだけ取得
                }
            }

            if (selectedImage != null)
            {
                Debug.Log("カーソルが重なっているImage: " + selectedImage.name);
                selectedImage.color = Color.yellow; // 例: 色を変える
            }
            else
            {
                Debug.Log("カーソルが重なるImageはありません。");
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
