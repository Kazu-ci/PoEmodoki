using System.Xml.Serialization;      //Unityのエディター機能を使うため
using UnityEditor;
using UnityEngine;

public class StatusMonitorWindow : EditorWindow
{

    //メニューバーに項目を追加
    //Tool/Status Monitor という項目が追加される
    [MenuItem("Tools/Status Monitor")]
    public static void ShowWindow()
    {
        //ウィンドウを開く
        //GetWindow<クラス名>("ウィンドウのタイトル")
        GetWindow<StatusMonitorWindow>("StatusMonitor");
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
    //ヒエラルキーで選択したオブジェクトを更新
    private void UpDateTarget()
    {
        selectedTarget = null;
        serializeStatus = null;

        GameObject selectedObj = Selection.activeGameObject;

        if(selectedObj != null)
        {
            selectedTarget = selectedObj.GetComponent<IStatusView>();
            if(selectedTarget != null)
            {
                serializeStatus = selectedTarget.GetSerializedBaseStatus();
            }
        }
        Repaint();
    }

    //ウィンドウに表示する内容を描画する
    void OnGUI()
    {
        //タイトルラベルの設定
        GUILayout.Label("GameStatus",EditorStyles.boldLabel);

        if(selectedTarget == null)
        {
            EditorGUILayout.LabelField("対象を選択してください");
            return;
        }
        //-----------------------------------------------------------------------------
        //読み取り専用
        EditorGUI.BeginDisabledGroup(true);

        if(Application.isPlaying)
        {
            selectedTarget.DrawRunningStatusGUI();
        }
        else
        {
            EditorGUILayout.LabelField("実行されてないお");
        }

        EditorGUILayout.Space(15);

        //基礎設計値を表示
        if(serializeStatus != null)
        {
            //＄これ便利
            GUILayout.Label($"基本設計値: {serializeStatus.targetObject.name} (監視のみ)", EditorStyles.boldLabel);

            SerializedProperty iterator = serializeStatus.GetIterator();
            iterator.NextVisible(true);
            while(iterator.NextVisible(false))
            {
                EditorGUILayout.PropertyField(iterator,true);
            }
        }
        EditorGUI.EndDisabledGroup();


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}
