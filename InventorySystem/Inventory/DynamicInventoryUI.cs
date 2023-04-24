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

    // ������ ���� ���� ����
    public override void CreateSlots()
    {
        for (int i = inventoryObject.Slots.Count; i < inventoryObject.InventoryMaxSize; ++i)
        {
            inventoryObject.Slots.Add(new InventorySlot());
        }

        int showSlotNumber = inventoryObject.NumberOfItem > _minInventorySize ? inventoryObject.NumberOfItem : _minInventorySize;

        // �κ��丮 �ּ� 50ĭ ǥ��, 50ĭ �̻��̶�� �ִ� ������ �ٱ����� ǥ��
        for (int i = 0; i < Mathf.CeilToInt((float)showSlotNumber / numberOfColumn); ++i)
        {
            GameObject go = Instantiate(slotsPrefab, Vector3.zero, Quaternion.identity, transform);

            for (int j = 0; j < numberOfColumn; ++j)
            {
                inventoryObject.Slots[i * numberOfColumn + j].slotUI = go.transform.GetChild(j).gameObject;
            }
            go.name += ": " + i;
        }

        // TODO : ������ ������ ���� ���� �ʱ� ǥ�� �κ��丮 Ȯ��
    }

    // ������ ����� ä���
    public void ArrangeItem()
    {
        if (inventoryObject.EmptySlotCount <= 0) return;

        // ����ִ� ������ ���� ���Ժ��� üũ
        for (int i = inventoryObject.GetEmptySlotIndex() + 1; i < inventoryObject.Slots.Count; ++i)
        {
            if (inventoryObject.Slots[i].itemData.id > -1)
            {
                inventoryObject.SwapItems(inventoryObject.Slots[i], inventoryObject.GetEmptySlot());
            }
        }
    }

    // TODO : �κ��丮 Ȯ��, ��� ���� ��� ����

    #endregion Methods
}