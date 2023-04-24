using UnityEngine;

public class DynamicInventoryUI : InventoryUI
{
    #region Variables
    [SerializeField]
    protected GameObject slotsPrefab;

    [Min(1), SerializeField]
    protected int numberOfColumn = 5;

    private readonly int _minInventorySize = 50;

    #endregion Variables

    #region Methods

    // 아이템 슬롯 최초 생성
    public override void CreateSlots()
    {
        for (int i = inventoryObject.Slots.Count; i < inventoryObject.InventoryMaxSize; ++i)
        {
            inventoryObject.Slots.Add(new InventorySlot());
        }

        int showSlotNumber = inventoryObject.NumberOfItem > _minInventorySize ? inventoryObject.NumberOfItem : _minInventorySize;

        // 인벤토리 최소 50칸 표시, 50칸 이상이라면 최대 아이템 줄까지만 표시
        for (int i = 0; i < Mathf.CeilToInt((float)showSlotNumber / numberOfColumn); ++i)
        {
            GameObject go = Instantiate(slotsPrefab, Vector3.zero, Quaternion.identity, transform);

            for (int j = 0; j < numberOfColumn; ++j)
            {
                inventoryObject.Slots[i * numberOfColumn + j].slotUI = go.transform.GetChild(j).gameObject;
            }
            go.name += ": " + i;
        }

        // TODO : 보관된 아이템 수에 따라 초기 표시 인벤토리 확장
    }

    // 아이템 빈공간 채우기
    public void ArrangeItem()
    {
        if (inventoryObject.EmptySlotCount <= 0) return;

        // 비어있는 슬롯의 다음 슬롯부터 체크
        for (int i = inventoryObject.GetEmptySlotIndex() + 1; i < inventoryObject.Slots.Count; ++i)
        {
            if (inventoryObject.Slots[i].itemData.id > -1)
            {
                inventoryObject.SwapItems(inventoryObject.Slots[i], inventoryObject.GetEmptySlot());
            }
        }
    }

    // TODO : 인벤토리 확장, 축소 등의 기능 구현

    #endregion Methods
}