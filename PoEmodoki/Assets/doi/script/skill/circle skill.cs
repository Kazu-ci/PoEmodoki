using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class circleskill : MonoBehaviour
{
    public GameObject objectToSpawn;
    public GameObject obj;
    private const float Y_OFFSET = 0.1f;
    public KeyCode spawnKey = KeyCode.Q;
    float atk;
    float speed;
    float Ct;
    float hp;
   
    
    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            Spawn();
        }
       
    }


    void Spawn()
    {
        Vector3 playerPosition = obj.transform.position;
        Vector3 spawnPosition = new Vector3(playerPosition.x, playerPosition.y + Y_OFFSET, playerPosition.z);
        Instantiate(objectToSpawn, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
    }
}
