using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    #region Variables

    public int id = -1;
    public string name;

    [SerializeField] private ItemType _type;
    public ItemStat[] stats;
    [SerializeField] public int _numberOfJewelSlot;
    [SerializeField] public ItemJewelSlot[] _jewelSlots;
    [SerializeField] private int _enhanceRank = 0;
    [SerializeField] private int _itemRank = 0;
    [SerializeField] private int _starRank = 0;
    [SerializeField] private bool _isStarItem = false;
    [SerializeField] private bool _isEvaluated = true;
    [SerializeField] private bool _isTradable = false;

    #endregion Variables

    #region Properties

    public int StarRank => _starRank;
    public int ItemRank => _itemRank;
    public bool IsTradable => _isTradable;
    public bool IsStarItem => _isStarItem;
    public bool IsEvaluated => _isEvaluated;
    public ItemType Type => _type;

    #endregion Properties

    public ItemData()
    {
        id = -1;
        name = "";
    }

    public ItemData(ItemObject itemObject)
    {
        id = itemObject.itemData.id;
        name = itemObject.itemData.name;
        _type = itemObject.itemData.Type;

        CreateItemRank(itemObject);
        CreateStarItem(itemObject);

        stats = new ItemStat[itemObject.itemData.stats.Length];
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i] = new ItemStat(itemObject.itemData.stats[i].DefaultValue, _itemRank, _starRank)
            {
                type = itemObject.itemData.stats[i].type
            };
        }

        // �ɼǼ� ���� : 1~4��
        _jewelSlots = new ItemJewelSlot[CreateItemSlotNumber()];
    }

    #region Methods

    private void CreateItemRank(ItemObject itemObject)
    {
        // CȮ�� : �� 15%
        // BȮ�� : �� 30%
        // AȮ�� : �� 55%
        // SȮ�� : 0.1% (����ġ)
        // RSȮ�� : 0.01% (����ġ)

        if ((int)itemObject.highestItemRank == 0)
        {
            _itemRank = 0;
        }
        else if ((int)itemObject.highestItemRank == 1)
        {
            float randNum = UnityEngine.Random.Range(0, 100f);

            if (randNum < 35) _itemRank = 0;
            else _itemRank = 1;
        }
        else if ((int)itemObject.highestItemRank == 2)
        {
            float randNum = UnityEngine.Random.Range(0, 100f);

            if (randNum < 15) _itemRank = 0;
            else if (randNum < 45) _itemRank = 1;
            else _itemRank = 2;
        }
        else if ((int)itemObject.highestItemRank == 3)
        {
            float randNum = UnityEngine.Random.Range(0, 100f);

            if (randNum < 15) _itemRank = 0;
            else if (randNum < 45) _itemRank = 1;
            else if (randNum < 99.9f) _itemRank = 2;
            else _itemRank = 3;

        }
        else if ((int)itemObject.highestItemRank == 4)
        {
            float randNum = UnityEngine.Random.Range(0, 100f);

            if (randNum < 15) _itemRank = 0;
            else if (randNum < 45) _itemRank = 1;
            else if (randNum < 99.89f) _itemRank = 2;
            else if (randNum < 99.9f) _itemRank = 3;
            else _itemRank = 4;
        }
        else
        {
            Debug.Log("���� RS ��� �̻��� ������ ��� Ȯ���� �����Ǿ� ���� ����");
        }
    }

    private void CreateStarItem(ItemObject itemObject)
    {
        if (!itemObject.isAbleToStarItem) return;

        // TODO: �÷��̾ ������ ������ ������ Ȯ�� 100%

        // �÷��̾ �Ϲ� �ʵ忡 ������ Ȯ�� 50% (���� Ȯ�� ����ġ �� 1%)

        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            _isStarItem = true;
            _isEvaluated = false;
        }
    }

    private int CreateItemSlotNumber()
    {
        float randNum = UnityEngine.Random.Range(0, 100f);

        // 4ĭ : 0.1%, 3ĭ : 1%, 2ĭ : 48.9%, 1ĭ : 50% (����ġ)

        if (randNum < 0.1f) return 4;
        else if (randNum < 1) return 3;
        else if (randNum < 50) return 2;
        else return 1;
    }

    public void EvaluateItemNormal()
    {
        if (!_isStarItem || _isEvaluated) return;

        float randNum = UnityEngine.Random.Range(0, 100f);

        // ���ø�ī Ȯ�� : 44.74% (? 0.02% �Ҽ��� ��° �ڸ�?)

        if (randNum < 0.04f) _starRank = 10;
        else if (randNum < 0.26f) _starRank = 9;
        else if (randNum < 0.71f) _starRank = 8;
        else if (randNum < 1.6f) _starRank = 7;
        else if (randNum < 2.94f) _starRank = 6;
        else if (randNum < 6.07f) _starRank = 5;
        else if (randNum < 12.78f) _starRank = 4;
        else if (randNum < 28.43f) _starRank = 3;
        else if (randNum < 55.26f) _starRank = 2;
        else _starRank = 1;

        _isEvaluated = true;

        UpdateItemStats();
    }

    public void EvaluateItemAdvanced()
    {
        if (!_isStarItem || _isEvaluated) return;

        float randNum = UnityEngine.Random.Range(0, 100);

        if (randNum < 0.25f) _starRank = 10;
        else if (randNum < 0.63f) _starRank = 9;
        else if (randNum < 1.38f) _starRank = 8;
        else if (randNum < 2.38f) _starRank = 7;
        else if (randNum < 3.63f) _starRank = 6;
        else if (randNum < 12.39f) _starRank = 5;
        else if (randNum < 24.91f) _starRank = 4;
        else if (randNum < 49.94f) _starRank = 3;
        else if (randNum < 74.97f) _starRank = 2;
        else _starRank = 1;

        _isEvaluated = true;

        UpdateItemStats();
    }

    private void UpdateItemStats()
    {
        for (int i = 0; i < stats.Length; ++i)
        {
            stats[i].GenerateValue(_itemRank, _starRank);
            stats[i].EnhanceItem(_enhanceRank);
        }
    }

    #endregion Methods
}
