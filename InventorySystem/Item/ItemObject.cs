using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/New Item")]
public class ItemObject : ScriptableObject
{
    #region Variables

    public int equipableLevel = 1;
    public ItemRank highestItemRank;
    public CharacterClass[] allowedClasses;
    public bool isAbleToStarItem = true;
    public bool stackable;

    public Sprite icon;
    public GameObject modelPrefab;

    public ItemData itemData = new();

    public List<string> boneNames = new();

    [TextArea(15, 20)]
    public string description;

    #endregion Variables


    #region Unity Methods
    // 캐릭터의 모델링을 변경할 때 사용
    private void OnEnable()
    {
        boneNames.Clear();
        if (modelPrefab == null || modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>() == null)
        {
            return;
        }

        SkinnedMeshRenderer renderer = modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
        var bones = renderer.bones;

        foreach (var t in bones)
        {
            boneNames.Add(t.name);
        }
    }

    #endregion Unity Methods

    #region Methods

    public ItemData CreateItem()
    {
        ItemData newItem = new ItemData(this);
        return newItem;
    }

    #endregion Methods
}