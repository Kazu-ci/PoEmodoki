using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform target;                  // プレイヤーのTransformを設定
    public Vector3 offset = new Vector3(0, 3f, -6f); // プレイヤーを基準としたカメラの位置
    public float smoothSpeed = 5f;          // 追従の速度（値が大きいほど速く追従）

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Targetが設定されていません！");
            return;
        }

        Vector3 desiredPosition = target.position + target.rotation * offset;

        // 2. スムーズ追従 (Time.deltaTimeを使用し、滑らかさを維持)
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;

        // 3. カメラをターゲットに向ける
        // Y軸を固定しない場合、LookAt(target) で十分です。
        Vector3 lookPos = target.position + Vector3.up * 1.5f;
        transform.LookAt(lookPos);
    }
}
