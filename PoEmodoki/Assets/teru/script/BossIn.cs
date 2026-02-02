using UnityEngine;

public class BossIn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerCon>();
        if (player != null&&GameCon.Instance.currentState==GameCon.GameState.Combat)
        {

        }
    }
}
