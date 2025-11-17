using TMPro;
using UnityEngine;

public class playerfollow : MonoBehaviour
{
    private const float Y_OFFSET = 0.1f;
    public GameObject obj;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = obj.transform.position;
        Vector3 targetPosition = new Vector3(playerPosition.x, Y_OFFSET, playerPosition.z);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.MovePosition(targetPosition);
    }
}
