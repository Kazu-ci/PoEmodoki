using Unity.VisualScripting;
using UnityEngine;

public class TestDamage: MonoBehaviour
{
    [SerializeField] BossEnemy boss;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boss=GetComponent<BossEnemy>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DamageData dmgdata=new DamageData();
            dmgdata.damageAmount = 10;
            boss.TakeDamage(dmgdata);
        }
    }
}
