using UnityEngine;
using System.Collections.Generic;
public class StateMachine<TOwner>
{
    /// <summary>
    ///�X�e�[�g���N���X
    ///�e�X�e�[�g�͂��̃N���X���p��
    public abstract class StateBase
    {
        public StateMachine<TOwner> StateMachine;
        protected TOwner Owner => StateMachine.Owner;
        //�X�e�[�}�V�����̏����S
        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        public virtual void OnEnd() { }



    }
    private TOwner Owner;
    private StateBase _currentState; // ���݂̃X�e�[�g
    private StateBase _prevState;    // �O�̃X�e�[�g
    private readonly Dictionary<int, StateBase> _states = new Dictionary<int, StateBase>(); // �S�ẴX�e�[�g��`
    public StateBase CurrentState => _currentState;//���݂̃X�e�[�g�Q��
    public virtual void OnCollision(Collision collision) { }//�ڐG����
    /// <summary>
    /// �R���X�g���N�^
    /// </summary>
    /// <param name="owner">StateMachine���g�p����Owner</param>
    public StateMachine(TOwner owner)
    {
        Owner = owner;
    }
    /// <summary>
    /// �X�e�[�g��`�o�^
    /// �X�e�[�g�}�V����������ɂ��̃��\�b�h���Ă�
    /// </summary>
    /// <param name="stateId">�X�e�[�gID</param>
    /// <typeparam name="T">�X�e�[�g�^</typeparam>
    public void Add<T>(int stateId) where T : StateBase, new()
    {
        if (_states.ContainsKey(stateId))
        {
            return;
        }
        // �X�e�[�g��`��o�^
        var newState = new T
        {
            StateMachine = this
        };
        _states.Add(stateId, newState);
    }

}
