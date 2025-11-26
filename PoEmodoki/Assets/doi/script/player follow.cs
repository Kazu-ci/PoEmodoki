using TMPro;
using UnityEngine;

public class playerfollow : MonoBehaviour
{
    [SerializeField]  GameObject objprefab;
    [SerializeField] GameObject player;
    [SerializeField] GameObject Particle;
    private GameObject obj;
    public float smoothSpeed = 5f;
    public KeyCode spawnKey = KeyCode.Q;
    Vector3 ppp;
    Vector3 playerPosition;
    int count = 0;
    bool alive = false; 
    private void Start()
    {
        
       
       
    }
    void Update()
    {
        ppp = new(playerPosition.x, 0.1f, playerPosition.z);
        if (Input.GetKeyDown(spawnKey) && alive == false)
        {
            obj = Instantiate(objprefab, ppp, Quaternion.Euler(90, 0, 0));
            Instantiate(Particle);
        }
        if (count > 10*60)
        {
            Destroy(obj);
            count = 0;
        }
        Debug.Log(alive);
    }
    void LateUpdate()
    {
        PlayerPositionget();
        if(obj != null)
        {
            alive = true;
            Debug.Log("objの位置" + obj.transform.position);
            Vector3 targetPosition = new Vector3(playerPosition.x, 0.1f, playerPosition.z);

            Vector3 smoothedPosition = Vector3.Lerp(
                obj.transform.position, // 現在の位置 (A)
                targetPosition,     // 目標の位置 (B)
                smoothSpeed * Time.deltaTime // 補間率 (t)
            );
            obj.transform.position = smoothedPosition;
            ++count;
        }
        else
        {
            alive = false;

        }
    }

     void PlayerPositionget()
    {
        playerPosition = player.transform.position;
    }
}
