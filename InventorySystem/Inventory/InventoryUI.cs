using SingletonPattern;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public abstract class InventoryUI : MonoBehaviour
{
    #region Variables

    public InventoryObject inventoryObject;

    #endregion Variables

    #region Unity Methods

    private void Awake()
    {
        CreateSlots();

        for (int i = 0; i < inventoryObject.Slots.Count; i++)
        {
            inventoryObject.Slots[i].OnPostUpdate += OnPostUpdate;
        }
    }

    protected virtual void Start()
    {
        for (int i = 0; i < inventoryObject.Slots.Count; ++i)
        {
            if (inventoryObject.Slots[i].slotUI != null) inventoryObject.Slots[i].UpdateSlot(inventoryObject.Slots[i].itemData, inventoryObject.Slots[i].amount);
        }
    }

    #endregion Unity Methods

    #region Methods

    public abstract void CreateSlots();
    
    // 인벤토리 슬롯의 아이템 아이콘, 개수 적용
    public void OnPostUpdate(InventorySlot slot)
    {
        if (slot == null || slot.slotUI == null) return;

        slot.slotUI.transform.GetChild(0).GetComponent<Image>().sprite = slot.itemData.id < 0 ? null : slot.ItemObject.icon;
        slot.slotUI.transform.GetChild(0).GetComponent<Image>().color = slot.itemData.id < 0 ? Color.clear : Color.white;
        if (slot.slotUI.transform.GetChild(1).GetComponent<Image>() != null)
        {
            slot.slotUI.transform.GetChild(1).GetComponent<Image>().sprite = slot.itemData.id < 0 ? null : UIManager.Instance.itemRankImage[slot.itemData.ItemRank];
            slot.slotUI.transform.GetChild(1).GetComponent<Image>().color = slot.itemData.id < 0 ? Color.clear : Color.white;
        }

        // PlayerInventory
        if (slot.slotUI.transform.childCount > 4)
        {
            if (slot.itemData.id < 0)
            {
                slot.slotUI.transform.GetChild(2).GetComponent<Image>().color = Color.clear;
                slot.slotUI.transform.GetChild(3).GetComponent<Image>().color = Color.clear;
            }

            if (slot.itemData.IsStarItem && !slot.itemData.IsEvaluated)
            {
                slot.slotUI.transform.GetChild(2).GetComponent<Image>().color = Color.white;
                slot.slotUI.transform.GetChild(3).GetComponent<Image>().color = Color.clear;
            }
            else if (slot.itemData.IsStarItem && slot.itemData.IsEvaluated)
            {
                slot.slotUI.transform.GetChild(2).GetComponent<Image>().color = Color.clear;

                slot.slotUI.transform.GetChild(3).GetComponent<Image>().sprite = UIManager.Instance.starRankImage[slot.itemData.StarRank];
                slot.slotUI.transform.GetChild(3).GetComponent<Image>().color = Color.white;
            }
            else if (!slot.itemData.IsStarItem)
            {
                slot.slotUI.transform.GetChild(2).GetComponent<Image>().color = Color.clear;
                slot.slotUI.transform.GetChild(3).GetComponent<Image>().color = Color.clear;
            }
            }
            // PlayerEquipment
            else
        {
            if (slot.itemData.id < 0)
            {
                slot.slotUI.transform.GetChild(2).GetComponent<Image>().color = Color.clear;
            }

            if (slot.itemData.IsStarItem && slot.itemData.IsEvaluated)
            {
                slot.slotUI.transform.GetChild(2).GetComponent<Image>().sprite = UIManager.Instance.starRankImage[slot.itemData.StarRank];
                slot.slotUI.transform.GetChild(2).GetComponent<Image>().color = Color.white;
            }
            else
            {
                slot.slotUI.transform.GetChild(2).GetComponent<Image>().color = Color.clear;
            }
        }

        if (slot.slotUI.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            slot.slotUI.GetComponentInChildren<TextMeshProUGUI>().text = slot.itemData.id < 0 ? string.Empty : (slot.amount == 1 ? string.Empty : slot.amount.ToString("n0"));
        }
    }

    #endregion Methods
}