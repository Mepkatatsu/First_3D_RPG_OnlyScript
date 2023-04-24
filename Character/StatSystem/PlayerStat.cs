using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public InventoryObject equipment;
    public StatsObject playerStats;

    private void OnEnable()
    {
        playerStats.OnChangedStats += OnChangedStats;

        if (equipment != null && playerStats != null)
        {
            foreach (InventorySlot slot in equipment.Slots)
            {
                slot.OnPreUpdate += OnRemoveItem;
                slot.OnPostUpdate += OnEquipItem;
            }
        }
    }

    private void OnDisable()
    {
        playerStats.OnChangedStats -= OnChangedStats;

        if (equipment != null && playerStats != null)
        {
            foreach (InventorySlot slot in equipment.Slots)
            {
                slot.OnPreUpdate -= OnRemoveItem;
                slot.OnPostUpdate -= OnEquipItem;
            }
        }
    }

    #region Methods

    // �������� ���� ������ �� �����Ǿ��ִ� �������� �ɷ�ġ�� ��
    private void OnRemoveItem(InventorySlot slot)
    {
        if (slot.ItemObject == null)
        {
            return;
        }

        foreach (ItemStat stat in slot.itemData.stats)
        {
            foreach (Attribute attribute in playerStats.attributes)
            {
                if (attribute.type == stat.type)
                {
                    attribute.value.RemoveModifier(stat);
                }
            }
        }
    }

    // �������� ������ �� ������ �������� �ɷ�ġ�� ����
    private void OnEquipItem(InventorySlot slot)
    {
        if (slot.ItemObject == null)
        {
            return;
        }

        foreach (ItemStat stat in slot.itemData.stats)
        {
            foreach (Attribute attribute in playerStats.attributes)
            {
                if (attribute.type == stat.type)
                {
                    attribute.value.AddModifier(stat);
                }
            }
        }
    }

    // ������ ����� �� ĳ���� ������ ǥ��� UI ���� (���� �ش� �κ� �̱���)
    private void OnChangedStats(StatsObject statsObject)
    {
        
    }

    #endregion Methods
}
