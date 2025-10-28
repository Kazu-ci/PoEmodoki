using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerCon : MonoBehaviour
{

    [Header("�v���C���[�̈ړ��ݒ�")]
    [SerializeField] private float _moveFroce = 5f;
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody _rigidbody;
    private InputSystem_Actions _inputActions;
    private Vector2 _moveInputValues;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //�A�N�V�����̃C���X�^���X��
        _inputActions = new InputSystem_Actions();
        //Action�C�x���g�̓o�^
        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        //�����ǉ�
        //=============================================

        _inputActions.Enable();
    }

    private void OnDestroy()
    {
        //���g��Instance������Action�N���X��IDisposable���������Ă�̂ŁADispose���ĊJ������K�v������
        _inputActions?.Dispose();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        //MoveAction�̓��͎擾
        _moveInputValues = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //�ړ������i���j
        _rigidbody.AddForce(new Vector3(_moveInputValues.x * _moveFroce, 0, _moveInputValues.y * _moveFroce));
    }
}
