using UnityEngine;

public class TextObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string targetBlockName;

    // プロパティでブロック名を外部（GameCon）から読み取れるようにする
    public string TargetBlockName => targetBlockName;


    public void OnInteract(PlayerCon player)
    {
        Debug.Log("oppai");
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // GameConに「自分がいま操作対象である」と伝える
            GameCon.Instance.RegisterInteractable(this);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // GameConの操作対象から自分を外す
            GameCon.Instance.UnregisterInteractable(this);
            Debug.Log("oppaijin");

        }
    }
}
