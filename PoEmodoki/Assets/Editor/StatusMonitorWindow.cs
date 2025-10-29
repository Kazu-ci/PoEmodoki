using UnityEngine;
using UnityEditor;      //Unity�̃G�f�B�^�[�@�\���g������

public class StatusMonitorWindow : EditorWindow
{

    //���j���[�o�[�ɍ��ڂ�ǉ�
    //Tool/Status Monitor �Ƃ������ڂ��ǉ������
    [MenuItem("Tools/Status Monitor")]
    public static void ShowWindow()
    {
        //�E�B���h�E���J��
        //GetWindow<�N���X��>("�E�B���h�E�̃^�C�g��")
        GetWindow<StatusMonitorWindow>("StatusMonitor");
    }

    //�E�B���h�E�ɕ\��������e��`�悷��
    void OnGUI()
    {
        //�^�C�g�����x���̐ݒ�
        GUILayout.Label("GameStatus",EditorStyles.boldLabel);

        //�Q�[�������s�����m�F
        if(Application.isPlaying)
        {
            //�V���O���g�������邩�m�F
            if(test.Instance != null)
            {
                //�X�e�[�^�X��\��
                EditorGUILayout.IntField("�G��HP:", test.Instance.EnemyHp);
                EditorGUILayout.IntField("�G�̍U����:", test.Instance.EnemyAtk);
                EditorGUILayout.IntField("�G�̖h���:", test.Instance.EnemyDefense);
                EditorGUILayout.FloatField("�G�̈ړ����x:",test.Instance.EnemySpeed);
                EditorGUILayout.FloatField("�G�̍U�����x:", test.Instance.EnemyAtkSpeed);
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
