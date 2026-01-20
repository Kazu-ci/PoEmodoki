using UnityEngine;
using UnityEngine.UI;
public class UIskill : MonoBehaviour
{
    [SerializeField] Slots slotdata;
    [SerializeField] Image slot1;
    [SerializeField] Image slot2;
    [SerializeField] Image slot3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(slotdata.slot1!=null)slot1.sprite = slotdata.slot1.sprite;
        if (slotdata.slot2!= null) slot2.sprite = slotdata.slot2.sprite;
        if (slotdata.slot3 != null) slot3.sprite = slotdata.slot3.sprite;

    }

    // Update is called once per frame
    void Update()
    {
        if (slotdata.slot1 != null) slot1.sprite = slotdata.slot1.sprite;
        if (slotdata.slot2 != null) slot2.sprite = slotdata.slot2.sprite;
        if (slotdata.slot3 != null) slot3.sprite = slotdata.slot3.sprite;
    }
}
