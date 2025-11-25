using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;




public class PlayerCon : MonoBehaviour,IStatusView
{
    [SerializeField]PlayerStatus player;
    [SerializeField]SerializedObject sPlayerStatus;
    [SerializeField] PlayerInput PlayerInput;
    public InputAction action;
    int MaxHP;
    int Defense;
    int MP;
    float MoveSpeed;
    float AtkSpeed;
    float CastSpeed;
    float Length;
    float ElementDefense;
    float Critical;

    
    
    private void Awake()
    {
        MaxHP = player.PlayerHp;
        Defense = player.PlayerDefense;
        MP = player.PlayerMp;
        MoveSpeed = player.PlayerSpeed;
        AtkSpeed = player.PlayerAtkSpeed;
        CastSpeed = player.PlayerCastSpeed;
        Length = player.PlayerLength;
        ElementDefense = player.PlayerElementDefense;
        Critical = player.PlayerCritical;
    }
    void Update()
    {
        //à⁄ìÆèàóùÅiâºÅj
        MovePlayer();
        
    }

    void MovePlayer()
    {
        var input = PlayerInput.actions["Move"].ReadValue<Vector2>();
        transform.Translate(new Vector3(input.x,0,input.y)*MoveSpeed);
    }

    public void DrawRunningStatusGUI()
    {

    }

    public SerializedObject GetSerializedBaseStatus()
    {
        return sPlayerStatus;
    }
}
