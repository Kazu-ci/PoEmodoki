using UnityEngine;

public class BossIn : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerCon>();
        if (player != null&&GameCon.Instance.currentState==GameCon.GameState.Combat)
        {
            GameCon.Instance.BossIn();
            Destroy(gameObject);
        }
    }
}
