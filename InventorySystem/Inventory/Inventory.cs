using System;
using System.Collections.Generic;

[Serializable]
public class Inventory
{
    #region Variables

    public int inventoryMaxSize = 50;   // DynamicInventoryUI에서 사용
    public int numberOfItem = 0;
    public List<InventorySlot> slots;

    #endregion Variables

    #region Properties
    #endregion Properties

    #region Methods

    public void CreateInitialSlots()
    {
        slots = new List<InventorySlot>(inventoryMaxSize);
    }

    public void Clear()
    {
        foreach (InventorySlot slot in slots)
        {
            slot.UpdateSlot(new ItemData(), 0);
        }
    }

    #endregion Methods
}