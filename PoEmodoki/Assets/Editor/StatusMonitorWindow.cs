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
        //シングルトンがあるか確認
        if(Application.isPlaying && test.Instance != null)
        {
            //ステータスを表示
            //敵
            EditorGUILayout.IntField("敵のHP:", test.Instance.EnemyHp);
            EditorGUILayout.IntField("敵の攻撃力:", test.Instance.EnemyAtk);
            EditorGUILayout.IntField("敵の防御力:", test.Instance.EnemyDefense);
            EditorGUILayout.FloatField("敵の移動速度:", test.Instance.EnemySpeed);
            EditorGUILayout.FloatField("敵の攻撃速度:", test.Instance.EnemyAtkSpeed);
            EditorGUILayout.FloatField("敵の詠唱速度:", test.Instance.EnemyCastSpeed);
            EditorGUILayout.FloatField("敵の射程:", test.Instance.EnemyLength);
            //プレイヤー
            EditorGUILayout.IntField("プレイヤーのHP:", test.Instance.PlayerHp);
            EditorGUILayout.IntField("プレイヤーの防御力:", test.Instance.PlayerDefense);
            EditorGUILayout.IntField("プレイヤーのMP:", test.Instance.PlayerMp);
            EditorGUILayout.FloatField("プレイヤーの移動速度:", test.Instance.PlayerSpeed);
            EditorGUILayout.FloatField("プレイヤーの攻撃速度:", test.Instance.PlayerAtkSpeed);
            EditorGUILayout.FloatField("プレイヤーの詠唱速度:", test.Instance.PlayerCastSpeed);
            EditorGUILayout.FloatField("プレイヤーの射程:", test.Instance.PlayerLength);
            EditorGUILayout.FloatField("プレイヤーの属性耐性:", test.Instance.PlayerElementDefense);
            EditorGUILayout.FloatField("プレイヤーのクリティカル:", test.Instance.PlayerCritical);
            //スキル
            EditorGUILayout.FloatField("スキルの攻撃力:", test.Instance.SkillAtk);
            EditorGUILayout.FloatField("スキルの移動速度:", test.Instance.SkillSpeed);
            EditorGUILayout.FloatField("スキルのクールタイム:", test.Instance.SkillCoolTime);
            EditorGUILayout.IntField("スキルの属性ダメージ:", test.Instance.SkillElementDmg);
            //武器
            EditorGUILayout.IntField("武器の攻撃力:", test.Instance.WeponAtk);
            EditorGUILayout.IntField("武器の属性ダメージ:", test.Instance.WeponElementDmg);
        }
        else
        {
            EditorGUILayout.LabelField("ゲームスタッツが初期化されてないゾ");
            EditorGUILayout.LabelField("ゲームが開始されてないゾ");
        }
        
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
