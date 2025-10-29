using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerCon : MonoBehaviour
{

    [Header("プレイヤーの移動設定")]
    [SerializeField] private float _moveFroce = 5f;
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody _rigidbody;
    private InputSystem_Actions _inputActions;
    private Vector2 _moveInputValues;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //アクションのインスタンス化
        _inputActions = new InputSystem_Actions();
        //Actionイベントの登録
        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        //随時追加
        //=============================================

        _inputActions.Enable();
    }

    private void OnDestroy()
    {
        //自身でInstance化したActionクラスはIDisposableを実装してるので、Disposeして開放する必要がある
        _inputActions?.Dispose();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        //MoveActionの入力取得
        _moveInputValues = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //移動処理（仮）
        _rigidbody.AddForce(new Vector3(_moveInputValues.x * _moveFroce, 0, _moveInputValues.y * _moveFroce));
    }
}
