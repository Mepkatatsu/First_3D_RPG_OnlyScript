using System;
using UnityEngine;

[Serializable]
public class ItemStat : IModifier
{
    #region Variables
    public AttributeType type;
    [SerializeField] private float defaultValue;
    private float _rankValue; // 강화 수치 적용 시 사용
    private float _rankStarvalue;
    [SerializeField] private float _finalValue;

    public float DefaultValue => defaultValue;

    #endregion Variables

    #region Methods

    public ItemStat(float defaultValue, int itemGrade, int starGrade)
    {
        this.defaultValue = defaultValue;
        GenerateValue(itemGrade, starGrade);
    }

    public void GenerateValue(int itemGrade, int starGrade)
    {
        switch (itemGrade)
        {
            case 0: _rankValue = defaultValue; break;
            case 1: _rankValue = defaultValue * 1.015f; break;
            case 2: _rankValue = defaultValue * 1.035f; break;
            case 3: _rankValue = defaultValue * 1.06f; break;
            case 4: _rankValue = defaultValue * 1.09f; break;
        }

        if (starGrade != 0)
        {
            if (starGrade < 7)
            {
                _rankStarvalue = _rankValue * (1.1f + starGrade * 0.1f);
            }
            else
            {
                _rankStarvalue = _rankValue * (1.7f + (starGrade - 6) * 0.15f);
            }
        }
        else _rankStarvalue = _rankValue;

        _finalValue = _rankStarvalue;
    }
    public float GetFinalValue()
    {
        // 고정 수치라면 반올림
        if (type == AttributeType.PhysicalAttack || type == AttributeType.MagicalAttack ||
            type == AttributeType.PhysicalDefense || type == AttributeType.MagicalDefense)
        {
            return Mathf.RoundToInt(_finalValue);
        }
        // % 수치라면 소수점 셋째자리에서 반올림
        else
        {
            return Mathf.Round(_finalValue * 100) / 100;
        }
    }

    public void EnhanceItem(int enhanceGrade)
    {
        switch (enhanceGrade)
        {
            case 0: break;
            case 1: _finalValue = _rankStarvalue + _rankValue * 0.2f; break;
            case 2: _finalValue = _rankStarvalue + _rankValue * 0.33f; break;
            case 3: _finalValue = _rankStarvalue + _rankValue * 0.46f; break;
            case 4: _finalValue = _rankStarvalue + _rankValue * 0.61f; break;
            case 5: _finalValue = _rankStarvalue + _rankValue * 0.77f; break;
            case 6: _finalValue = _rankStarvalue + _rankValue * 0.94f; break;
            case 7: _finalValue = _rankStarvalue + _rankValue * 1.12f; break;
            case 8: _finalValue = _rankStarvalue + _rankValue * 1.32f; break;
            case 9: _finalValue = _rankStarvalue + _rankValue * 1.52f; break;
            case 10: _finalValue = _rankStarvalue + _rankValue * 1.74f; break;
            case 11: _finalValue = _rankStarvalue + _rankValue * 1.97f; break;
            case 12: _finalValue = _rankStarvalue + _rankValue * 2.21f; break;
            case 13: _finalValue = _rankStarvalue + _rankValue * 2.46f; break;
            case 14: _finalValue = _rankStarvalue + _rankValue * 2.72f; break;
            case 15: _finalValue = _rankStarvalue + _rankValue * 3.0f; break;
        }
    }

    #endregion Methods

    #region IModifier interface

    public void AddValue(ref float v)
    {
        v += _finalValue;
    }

    #endregion IModifier interface
}
