using UnityEngine;
using UnityEngine.Rendering;
public class Enemy : MonoBehaviour
{
    //�e�X�e�[�^�X
    [Header("�X�e�[�^�X")]
    [SerializeField] protected float MaxHP;//�ő�HP
    [SerializeField] protected float Strength;//�U����
    [SerializeField] protected float AttackSpeed;//�U�����x
    [SerializeField] protected float AttackRange;//�U���˒�
    [SerializeField ] protected float AttackRate;//�U�����o
    [SerializeField] protected Texture[] textures;//�e�N�X�`��
    [SerializeField] protected float fov;//����p
    [SerializeField] protected GameObject thisobj;//�e�N�X�`���ύX�p
    protected float currentHP;//���݂�HP
    protected float Distance;//�v���C���[�Ƃ̋���
    protected GameObject weapon;//�h���b�v�A�C�e��
    protected Animator animator;//�A�j���[�V����
    protected bool AnimationEnd;//�A�j���[�V�����I���p�t���O
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
}
