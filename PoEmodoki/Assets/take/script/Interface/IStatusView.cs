using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//このインターフェイスを持つクラスはモニターウィンドウに数値を提供できる
public interface IStatusView
{
    //実行中のステータスを描画する
    void DrawRunningStatusGUI();

    //編集可能な基本設計値(ScriptableObject)を返す
    //編集するものが無ければnullを返す
#if UNITY_EDITOR
    SerializedObject GetSerializedBaseStatus();
#endif
}
