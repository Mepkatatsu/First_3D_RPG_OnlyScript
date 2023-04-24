using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats", menuName ="Stats System/New Character Stats")]
public class StatsObject : ScriptableObject
{
    #region Variables

    public Attribute[] attributes;

    public int level = 1;
    public int currentExp = 0;

    public Action<StatsObject> OnChangedStats;

    #endregion Variables

    #region Properties

    public float CurrentHP
    {
        get; set;
    }

    public float CurrentMP
    {
        get; set;
    }

    public float MaxHP
    {
        get
        {
            float maxHP = -1;
            foreach (Attribute attribute in attributes)
            {
                if (attribute.type == AttributeType.MaxHP)
                {
                    maxHP = attribute.value.ModifiedValue;
                }
            }
            return maxHP;
        }
    }

    public float MaxMP
    {
        get
        {
            float maxMP = -1;
            foreach (Attribute attribute in attributes)
            {
                if (attribute.type == AttributeType.MaxMP)
                {
                    maxMP = attribute.value.ModifiedValue;
                }
            }
            return maxMP;
        }
    }

    public float HPPercentage
    {
        get
        {
            float hp = CurrentHP;
            float maxHP = MaxHP;

            return (maxHP > 0 ? ((float)hp / (float)maxHP) : 0f);
        }
    }

    public float MPPercentage
    {
        get
        {
            float mp = CurrentMP;
            float maxMP = MaxMP;

            return (maxMP > 0 ? ((float)mp / (float)maxMP) : 0f);
        }
    }

    #endregion Properties

    #region Methods

    public void OnEnable()
    {
        InitializeAttribute();
    }

    public void InitializeAttribute()
    {
        foreach (Attribute attribute in attributes)
        {
            attribute.value = new ModifiableFloat(OnModifiedValue);
        }

        // 기본 스탯 설정(레벨별?)
        SetBaseValue(AttributeType.PhysicalAttack, 100);
        SetBaseValue(AttributeType.MagicalAttack, 100);
        SetBaseValue(AttributeType.PhysicalDefense, 100);
        SetBaseValue(AttributeType.MagicalDefense, 100);
        SetBaseValue(AttributeType.MaxHP, 1000);
        SetBaseValue(AttributeType.MaxMP, 100);

        CurrentHP = GetModifiedValue(AttributeType.MaxHP);
        CurrentMP = GetModifiedValue(AttributeType.MaxMP);
    }

    private void OnModifiedValue(ModifiableFloat value)
    {
        OnChangedStats?.Invoke(this);
    }

    public float GetBaseValue(AttributeType type)
    {
        foreach (Attribute attribute in attributes)
        {
            if (attribute.type == type)
            {
                return attribute.value.BaseValue;
            }
        }

        return -1;
    }

    public void SetBaseValue(AttributeType type, int value)
    {
        foreach (Attribute attribute in attributes)
        {
            if (attribute.type == type)
            {
                attribute.value.BaseValue = value;
            }
        }
    }

    public float GetModifiedValue(AttributeType type)
    {
        foreach (Attribute attribute in attributes)
        {
            if (attribute.type == type)
            {
                return attribute.value.ModifiedValue;
            }
        }

        return -1;
    }

    public float AddHealth(int value)
    {
        CurrentHP += value;

        if (CurrentHP < 0) CurrentHP = 0;

        OnChangedStats?.Invoke(this);

        return CurrentHP;
    }

    public float AddMana(int value)
    {
        CurrentMP += value;

        if (CurrentMP < 0) CurrentMP = 0;

        OnChangedStats?.Invoke(this);

        return CurrentMP;
    }

    #endregion Methods
}
