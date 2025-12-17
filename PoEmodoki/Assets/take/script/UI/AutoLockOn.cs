using System.Runtime.CompilerServices;
using UnityEngine;

public class AutoLockOn : MonoBehaviour
{
    [Header("çıìGê›íË")]
    [SerializeField] private float lockRange = 15f;
    [SerializeField] private LayerMask enemyLayer;
    [Header("UIê›íË")]
    [SerializeField] private RectTransform lockonUI;

    private Transform target;
    private bool islockon = false;
    void Start()
    {
        if(lockonUI != null) lockonUI.gameObject.SetActive(false);
    }
    void Update()
    {
            
    }

    void ToggleLockOn()
    {
        
    }

    void UnlockOn()
    {

    }
}