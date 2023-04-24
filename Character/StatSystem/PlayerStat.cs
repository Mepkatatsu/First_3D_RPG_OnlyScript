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

    // 아이템이 장착 해제될 때 장착되어있던 아이템의 능력치를 뺌
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

    // 아이템이 장착될 때 장착한 아이템의 능력치를 더함
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

    // 스탯이 변경될 때 캐릭터 스탯이 표기된 UI 변경 (현재 해당 부분 미구현)
    private void OnChangedStats(StatsObject statsObject)
    {
        
    }

    #endregion Methods
}
