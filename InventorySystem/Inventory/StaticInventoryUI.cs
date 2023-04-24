using UnityEngine;

public class StaticInventoryUI : InventoryUI
{
    public GameObject[] staticSlots = null;

    // 아이템 슬롯 등록
    public override void CreateSlots()
    {
        for (int i = 0; i < inventoryObject.Slots.Count; i++)
        {
            GameObject slotGO = staticSlots[i];

            inventoryObject.Slots[i].slotUI = slotGO;

            slotGO.name += ": " + i;
        }
    }
}