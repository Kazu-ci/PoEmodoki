using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("追従するプレイヤー")]
    public Transform Target;
    [Header("プレイヤーとの距離")]
    public Vector3 offset;
    void Start()
    {
        
    }
    //※カメラの追従はLateUpdateで
    void LateUpdate()
    {
        if(Target != null)
        {
            transform.position = Target.position + offset;
        }
    }
}
