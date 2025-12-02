using UnityEngine;

public class mine : MonoBehaviour
{
    [SerializeField] SkillStatus data;
    [SerializeField] GameObject player;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float atk = data.atk;
        rb = GetComponent<Rigidbody>();
    }
    public GameObject bomb;
    // Update is called once per frame
    void Update()
    {
       
    }
    void OnCollisionEnter(Collision collision)
    {
        //hp‚ðŒ¸‚ç‚·
        Instantiate(bomb, player.transform.position, Quaternion.Euler(-90,0,0));
        Destroy(gameObject);
    }
}
