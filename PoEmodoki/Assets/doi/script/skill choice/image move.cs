using UnityEngine;
using UnityEngine.UI;
public class imagemove : MonoBehaviour
{
    // 依存するRectTransformをInspectorで設定（publicフィールドまたはSerializeFieldを使う）
    public RectTransform imageRect;
    public RectTransform slot0;
    public RectTransform slot1;
    public RectTransform slot3;

    Vector2 length;
    private void Update()
    {
        Vector2 move = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.RightArrow) && imageRect.anchoredPosition.x < 350 && imageRect.anchoredPosition.x != slot3.anchoredPosition.x)
        {
            move.x = 150;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && imageRect.anchoredPosition.x > -400 && imageRect.anchoredPosition.x != slot1.anchoredPosition.x)
        {
            move.x = -150;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) )
        {
           if (imageRect.anchoredPosition.y >= 0)
            {
                move = slot1.anchoredPosition - imageRect.anchoredPosition;
            }
        }
        if (Input.GetKey(KeyCode.DownArrow) && imageRect.anchoredPosition.y == 120)
        {
            imageRect.anchoredPosition = slot0.anchoredPosition;
        }
        imageRect.anchoredPosition += move;

    }
}
