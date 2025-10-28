using UnityEngine;
using UnityEditor;      //Unityのエディター機能を使うため

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

    //ウィンドウに表示する内容を描画する
    void OnGUI()
    {
        //タイトルラベルの設定
        GUILayout.Label("GameStatus",EditorStyles.boldLabel);

        //ゲームが実行中か確認
        if(Application.isPlaying)
        {
            //シングルトンがあるか確認
            if(test.Instance != null)
            {
                //ステータスを表示
                EditorGUILayout.IntField("敵のHP:", test.Instance.EnemyHp);
                EditorGUILayout.IntField("敵の攻撃力:", test.Instance.EnemyAtk);
                EditorGUILayout.IntField("敵の防御力:", test.Instance.EnemyDefense);
                EditorGUILayout.FloatField("敵の移動速度:",test.Instance.EnemySpeed);
                EditorGUILayout.FloatField("敵の攻撃速度:", test.Instance.EnemyAtkSpeed);
            }

        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
