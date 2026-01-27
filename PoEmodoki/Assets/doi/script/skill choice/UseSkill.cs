using System.Collections.Generic;
using UnityEngine;

public class UseSkill : MonoBehaviour
{
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;
    [SerializeField] PlayerCon player;
    [SerializeField] Slots slotData;
    [SerializeField] SkillStatus Tossin;
    [SerializeField] SkillStatus AoE;
    [SerializeField] SkillStatus AOE;
    [SerializeField] SkillStatus Jump;
    [SerializeField] SkillStatus Blink;
    [SerializeField] SkillStatus mine;
    public List<Slots> skill1 = new();
    public List<Slots> skill2 = new();
    public List<Slots> skill3 = new();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(player.mySkills[1]);
        if(slotData.slot1.Skillname =="Tossin")
        {
            player.mySkills[0] = Tossin;
        }
        if (slotData.slot1.Skillname == "AoE")
        {
            player.mySkills[0] = AoE;
        }
        if (slotData.slot1.Skillname == "AOE")
        {
            player.mySkills[0] = AOE;
        }
        if (slotData.slot1.Skillname == "Jump")
        {
            player.mySkills[0] = Jump;
        }
        if (slotData.slot1.Skillname == "Blink")
        {
            player.mySkills[0] = Blink;
        }
        if (slotData.slot1.Skillname == "mine")
        {
            player.mySkills[0] = Blink;
        }
        //-----------------------------------------------
        if (slotData.slot2.Skillname == "Tossin")
        {
            player.mySkills[1] = Tossin;
        }
        if (slotData.slot2.Skillname == "AoE")
        {
            player.mySkills[1] = AoE;
        }
        if (slotData.slot2.Skillname == "AOE")
        {
            player.mySkills[1] = AOE;
        }
        if (slotData.slot2.Skillname == "Jump")
        {
            player.mySkills[1] = Jump;
        }
        if (slotData.slot2.Skillname == "Blink")
        {
            player.mySkills[1] = Blink;
        }
        if (slotData.slot2.Skillname == "mine")
        {
            player.mySkills[1] = Blink;
        }
        //-----------------------------------------------------
        if (slotData.slot3.Skillname == "Tossin")
        {
            player.mySkills[2] = Tossin;
        }
        if (slotData.slot3.Skillname == "AoE")
        {
            player.mySkills[2] = AoE;
        }
        if (slotData.slot3.Skillname == "AOE")
        {
            player.mySkills[2] = AOE;
        }
        if (slotData.slot3.Skillname == "Jump")
        {
            player.mySkills[2] = Jump;
        }
        if (slotData.slot3.Skillname == "Blink")
        {
            player.mySkills[2] = Blink;
        }
        if (slotData.slot3.Skillname == "mine")
        {
            player.mySkills[2] = Blink;
        }
    }
}
