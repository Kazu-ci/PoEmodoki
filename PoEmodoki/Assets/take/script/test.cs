using UnityEngine;

public class test : MonoBehaviour
{

    public static test Instance {  get; private set; }
    [Header("�G�̃X�e�[�^�X")]
    //�G�̃X�e�[�^�X
    public int EnemyHp;                 //�G��HP
    public int EnemyAtk;                //�G�̍U����
    public int EnemyDefense;            //�G�̖h���
    public float EnemySpeed;            //�G�̈ړ����x
    public float EnemyAtkSpeed;         //�G�̍U�����x
    public float EnemyCastSpeed;        //�G�̉r�����x
    public float EnemyLength;           //�G�̎˒�
    public float EnemyElementDefense;   //�G�̑����ϐ�
    [Header("�v���C���[�̃X�e�[�^�X")]
    //�v���C���[�̃X�e�[�^�X
    public int PlayerHp;                //�v���C���[��HP
    public int PlayerDefense;           //�v���C���[�̖h���
    public float PlayerSpeed;           //�v���C���[�̈ړ����x
    public int PlayerMp;                //�v���C���[��MP
    public float PlayerAtkSpeed;        //�v���C���[�̍U�����x
    public float PlayerCastSpeed;       //�v���C���[�̉r�����x
    public float PlayerLength;          //�v���C���[�̎˒�
    public float PlayerElementDefense;  //�v���C���[�̑����ϐ�
    public float PlayerCritical;        //�N���e�B�J��
    [Header("�X�L���̃X�e�[�^�X")]
    //�X�L���̃X�e�[�^�X
    public float SkillAtk;              //�X�L���̍U����
    public float SkillSpeed;            //�X�L���̔��ł����x
    public float SkillCoolTime;         //�X�L���̃N�[���^�C��
    public int SkillElementDmg;         //�X�L���̑����U����
    [Header("����̃X�e�[�^�X")]
    //����̃X�e�[�^�X
    public int WeponAtk;                //����̍U����
    public int WeponElementDmg;         //����̕���̑����_���[�W
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
