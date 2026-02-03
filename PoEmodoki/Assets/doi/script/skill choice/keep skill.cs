using System.Collections.Generic;
//using UnityEditor.SceneManagement;
using UnityEngine;

public class keepskill : MonoBehaviour
{
    [SerializeField] RectTransform slot1;
    [SerializeField] RectTransform slot2;
    [SerializeField] RectTransform slot3;
    [SerializeField] RectTransform ImageRect;
    [SerializeField] List<RectTransform> otherImages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
                    Debug.Log($"{image.name} Ç∆èdÇ»Ç¡ÇƒÇ¢Ç‹Ç∑ÅI");
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
