using UnityEngine;

public class Starts : MonoBehaviour
{
    [SerializeField] GameObject obj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(obj);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
