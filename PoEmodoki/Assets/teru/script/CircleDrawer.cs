using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CircleDrawer : MonoBehaviour
{
    [SerializeField] float radius;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        GizmosUtility.DrawWireRegularPolygon(64, transform.position, Quaternion.Euler(90.0f, 0.0f, 0.0f), radius);
    }
}
