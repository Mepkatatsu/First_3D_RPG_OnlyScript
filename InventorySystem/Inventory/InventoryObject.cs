using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[CreateAssetMenu(fileName = "New Invnetory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    #region Variables

    public ItemDatabaseObject database;

    [SerializeField]
    private Inventory _container = new();

    public Action<ItemObject> OnUseItem;

    #endregion Variables

    #region Properties

    public List<InventorySlot> Slots => _container.slots;
    public int InventoryMaxSize => _container.inventoryMaxSize;
    public int NumberOfItem => _container.numberOfItem;

    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            foreach (InventorySlot slot in Slots)
            {
                if (slot.itemData.id <= -1)
                {
                    counter++;
                }
            }

            return counter;
        }
    }

    #endregion Properties

    #region Methods

    public bool AddItem(ItemData item, int amount)
    {
        InventorySlot slot = FindItemInInventory(item);
        if (!database.FindItem(item.id).stackable || slot == null)
        {
            if (EmptySlotCount <= 0)
            {
                return false;
            }

            GetEmptySlot().UpdateSlot(item, amount);
        }
        else
        {
            slot.AddAmount(amount);
        }

        return true;
    }

    public InventorySlot FindItemInInventory(ItemData item)
    {
        return Slots.FirstOrDefault(i => i.itemData.id == item.id);
    }

    public bool IsContainItem(ItemObject itemObject)
    {
        return Slots.FirstOrDefault(i => i.itemData.id == itemObject.itemData.id) != null;
    }

    public InventorySlot GetEmptySlot()
    {
        return Slots.FirstOrDefault(i => i.itemData.id <= -1);
    }

    public int GetEmptySlotIndex()
    {
        for (int i = 0; i < Slots.Count; ++i)
        {
            if (Slots[i].itemData.id <= -1) return i;
        }
        return -1;
    }

    public void UseItem(InventorySlot slotToUse)
    {
        if (slotToUse.ItemObject == null || slotToUse.itemData.id < 0 || slotToUse.amount <= 0)
        {
            return;
        }

        ItemObject itemObject = slotToUse.ItemObject;
        slotToUse.UpdateSlot(slotToUse.itemData, slotToUse.amount - 1);

        OnUseItem.Invoke(itemObject);
    }

    public void SwapItems(InventorySlot slotA, InventorySlot slotB)
    {
        if (slotB.CanPlaceInSlot(slotA.itemData) && slotA.CanPlaceInSlot(slotB.itemData))
        {
            InventorySlot temp = new InventorySlot(slotB.itemData, slotB.amount);
            slotB.UpdateSlot(slotA.itemData, slotA.amount);
            slotA.UpdateSlot(temp.itemData, temp.amount);
        }
    }

    #endregion Methods

    #region Save/Load Methods
    public string savePath;

    [ContextMenu("Save")]
    public void Save()
    {
        #region Optional Save
        //string saveData = JsonUtility.ToJson(Container, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();
        #endregion

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, _container);
        stream.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            #region Optional Load
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), Container);
            //file.Close();
            #endregion

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < Slots.Count; i++)
            {
                Slots[i].UpdateSlot(newContainer.slots[i].itemData, newContainer.slots[i].amount);
            }
            stream.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        _container.Clear();
    }
    #endregion Save/Load Methods
}