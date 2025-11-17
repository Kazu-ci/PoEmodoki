using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform target;                  // プレイヤー
    public Vector3 offset = new Vector3(0, 3, -6);
    public float smoothSpeed = 0.15f;         // 追従の滑らかさ

    private void LateUpdate()
    {
        if (target == null) return;

        // ★ 目標位置
        Vector3 desiredPosition = target.position + offset;

        // ★ スムーズ追従
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // ★ LookAt をそのまま使うと揺れやすいので XZ 平面で向く
        Vector3 lookPos = new Vector3(target.position.x, target.position.y + 1.5f, target.position.z);
        transform.LookAt(lookPos);
    }
}
