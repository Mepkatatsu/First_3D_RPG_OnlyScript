using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModifiableFloat
{
    #region Variables

    [NonSerialized] private float _baseValue;
    [SerializeField] private float _modifiedValue;

    private event Action<ModifiableFloat> OnModifiedValue;

    private List<IModifier> _modifiers = new();

    #endregion Variables

    #region Properties

    public float BaseValue
    {
        get => _baseValue;
        set
        {
            _baseValue = value;
            UpdateModifiedValue();
        }
    }

    public float ModifiedValue
    {
        get => _modifiedValue;
        set => _modifiedValue = value;
    }

    #endregion Properties

    #region Methods

    public ModifiableFloat(Action<ModifiableFloat> method = null)
    {
        ModifiedValue = _baseValue;
        RegisterModEvent(method);
    }

    public void RegisterModEvent(Action<ModifiableFloat> method)
    {
        if (method != null)
        {
            OnModifiedValue += method;
        }
    }

    public void UnregisterModEvent(Action<ModifiableFloat> method)
    {
        if (method != null)
        {
            OnModifiedValue -= method;
        }
    }

    private void UpdateModifiedValue()
    {
        float valueToAdd = 0;
        foreach (IModifier modifier in _modifiers)
        {
            modifier.AddValue(ref valueToAdd);
        }

        ModifiedValue = _baseValue + valueToAdd;

        OnModifiedValue?.Invoke(this);
    }

    public void AddModifier(IModifier modifier)
    {
        _modifiers.Add(modifier);

        UpdateModifiedValue();
    }

    public void RemoveModifier(IModifier modifier)
    {
        _modifiers.Remove(modifier);
        UpdateModifiedValue();
    }

    #endregion Methods
}
