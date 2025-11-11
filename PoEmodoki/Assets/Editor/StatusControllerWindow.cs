using UnityEngine;
using UnityEditor;

public class StatusControllerWindow : EditorWindow
{
    [MenuItem("Tools/Status Controller")]

    public static void ControllerWindow()
    {
        GetWindow<StatusControllerWindow>("Controller Window");
    }

    private void OnGUI()
    {
        GUILayout.Label("編集", EditorStyles.boldLabel);
        if(Application.isPlaying && test.Instance != null)
        {
            int newHp = EditorGUILayout.IntField("敵のHP:", test.Instance.EnemyHp);
            if (newHp != test.Instance.EnemyHp)
            {
                test.Instance.EnemyHp = newHp;
            }
        }
        else
        {
            EditorGUILayout.LabelField("ゲームスタッツが初期化されてないゾ");
            EditorGUILayout.LabelField("ゲームが開始されてないゾ");
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
