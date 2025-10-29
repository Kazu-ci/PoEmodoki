using UnityEngine;
using UnityEngine.UI;
public class imagemove : MonoBehaviour
{
    public RectTransform imageRect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.RightArrow)&& imageRect.anchoredPosition.x < 350)
        {
            move.x = 150;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)&&imageRect.anchoredPosition.x > -400)
        {
            move.x = -150;
        }
        if (Input.GetKey(KeyCode.UpArrow) && imageRect.anchoredPosition.y < 0)
        {
            move.y = 150;
        }
        if (Input.GetKey(KeyCode.DownArrow) && imageRect.anchoredPosition.y > -150)
        {
            move.y = -150;
        }
        imageRect.anchoredPosition += move;
    }
}
