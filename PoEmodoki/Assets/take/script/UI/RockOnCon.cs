using UnityEngine;
using UnityEngine.UI;

public class RockOnCon : MonoBehaviour
{
    //自分のRectTransform
    [SerializeField]protected RectTransform rectTransform;
    //カーソルのイメージ
    [SerializeField]protected Image image;
    //ロックオン対象のTransform
    protected Transform LockOnTarget { get;set; }
    void Start()
    {
        image.enabled = false;
    }

    void Update()
    {
        if(image.enabled)
        {
            rectTransform.Rotate(0, 0, 1f);
            if (LockOnTarget != null)
            {
                Vector3 targetPoint = Camera.main.WorldToScreenPoint(LockOnTarget.position);
                rectTransform.position = targetPoint;
            }
            
        }
    }

    public void OnLockonStart(Transform target)
    {
        image.enabled = true;
        LockOnTarget = target;
    }
    public void OnLockonEnd()
    {
        image.enabled = false;
        LockOnTarget = null;
    }
}
