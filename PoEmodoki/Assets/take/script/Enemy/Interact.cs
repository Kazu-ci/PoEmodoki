using UnityEngine;

public class Interact : MonoBehaviour, IInteractable
{
    [SerializeField] Collider SoulColl;
    [SerializeField] Collider InteractColl;
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void OnInteract(PlayerCon player)
    {
        Debug.Log("プレイヤーが魂を拾った。");
        //プレイヤーが拾ったらアイテムを消す
        Destroy(gameObject);
    }
}
