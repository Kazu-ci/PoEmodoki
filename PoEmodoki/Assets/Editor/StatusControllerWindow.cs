using UnityEngine;
using UnityEditor;

public class StatusControllerWindow : EditorWindow
{
    [MenuItem("Tools/Status Controller")]

    
    public static void ControllerWindow()
    {
        GetWindow<StatusControllerWindow>("Controller Window");
    }

    private IStatusView selectedTarget;
    private SerializedObject serializeStatus;

    private void OnSelectionChange()
    {
        UpDateTarget();
    }
    private void OnFocus()
    {
        UpDateTarget();
    }

    private void UpDateTarget()
    {
        selectedTarget = null;
        serializeStatus = null;

        GameObject selectedObj = Selection.activeGameObject;

        if (selectedObj != null)
        {
            selectedTarget = selectedObj.GetComponent<IStatusView>();
            if (selectedTarget != null)
            {
                serializeStatus = selectedTarget.GetSerializedBaseStatus();
            }
        }
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("編集", EditorStyles.boldLabel);
        if(selectedTarget == null)
        {
            EditorGUILayout.LabelField("編集対象を選択してください");
            return;
        }

        //---------------------------------------------------------
        //基礎設計値を編集
        if(serializeStatus != null)
        {
            GUILayout.Label($"基本設計値: {serializeStatus.targetObject.name} (編集可)",EditorStyles.boldLabel);

            serializeStatus.Update();


            SerializedProperty iterator = serializeStatus.GetIterator();
            iterator.NextVisible(true);
            while (iterator.NextVisible(false))
            {
                EditorGUILayout.PropertyField(iterator, true);
            }

            serializeStatus.ApplyModifiedProperties();
        }
        else
        {
            EditorGUILayout.LabelField("編集可能な基本設計値がありません");
        }
        

    }
}
