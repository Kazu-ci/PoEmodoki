using UnityEngine;

public class mine : MonoBehaviour
{
    [SerializeField] SkillStatus data;
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
        Vector3 point = new Vector3(transform.position.x, transform.position.y * 4,transform.position.z);
        Instantiate(bomb, transform.position, Quaternion.Euler(-90,0,0));
        Destroy(gameObject);
    }
}
