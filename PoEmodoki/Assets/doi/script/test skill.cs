using UnityEngine;

public class testskill : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public GameObject obj;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Instantiate(obj);
        }
    }
}
