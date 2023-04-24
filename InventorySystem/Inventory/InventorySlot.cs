using SingletonPattern;
using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    #region Variables

    public ItemType[] AllowedItems = new ItemType[0];

    [NonSerialized] public InventoryObject parent;
    [NonSerialized] public GameObject slotUI;

    [NonSerialized] public Action<InventorySlot> OnPreUpdate;
    [NonSerialized] public Action<InventorySlot> OnPostUpdate;

    public ItemData itemData;
    public int amount;

    #endregion Variables

    #region Properties
    public ItemObject ItemObject
    {
        get
        {
            return itemData.id >= 0 ? UIManager.Instance.itemDatabase.FindItem(itemData.id) : null;
        }
    }

    #endregion Properties

    #region Methods
    public InventorySlot() => UpdateSlot(new ItemData(), 0);

    public InventorySlot(ItemData item, int amount) => UpdateSlot(item, amount);

    public void RemoveItem() => UpdateSlot(new ItemData(), 0);

    public void AddAmount(int value) => UpdateSlot(itemData, amount += value);

    public void UpdateSlot(ItemData itemData, int amount)
    {
        if (amount <= 0)
        {
            itemData = new ItemData();
        }

        OnPreUpdate?.Invoke(this);
        this.itemData = itemData;
        this.amount = amount;
        OnPostUpdate?.Invoke(this);
    }

    public bool CanPlaceInSlot(ItemData item) {
        if (AllowedItems.Length <= 0 || item == null || item.id < 0)
        {
            return true;
        }

        foreach (ItemType itemType in AllowedItems)
        {
            if (item.Type == itemType)
            {
                return true;
            }
        }

        return false;
    }

    #endregion Methods
}