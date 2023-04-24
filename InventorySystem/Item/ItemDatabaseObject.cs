using UnityEngine;

// id
// 1레벨 아이템 1000~
// 6레벨 아이템 2000~
// ...


[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject
{
    public ItemObject[] itemObjects;
    private int[] levelItemCount = new int[61]; // 데이터베이스에 등록된 레벨별 아이템 수 (~301레벨)

    private void OnEnable()
    {
        int currentLevelIndex;

        for (int i = 0; i < levelItemCount.Length; ++i)
        {
            levelItemCount[i] = 0;
        }

        for (int i = 0; i < itemObjects.Length; ++i)
        {
            currentLevelIndex = (itemObjects[i].equipableLevel - 1) / 5 + 1;
            itemObjects[i].itemData.id = currentLevelIndex * 1000 + levelItemCount[currentLevelIndex];
            levelItemCount[currentLevelIndex]++;
        }
    }

    #region Methods

    public ItemObject FindItem(int id)
    {
        for (int i = 0; i < itemObjects.Length; ++i)
        {
            if (id == itemObjects[i].itemData.id) return itemObjects[i];
        }
        Debug.Log("Can't Find Item / FindItem");
        return null;
    }

    public ItemObject GetRandomLevelItem(int minItemLevel, int maxItemLevel)
    {
        int minLevelIndex = (minItemLevel - 1) / 5 + 1;
        int maxLevelIndex = (maxItemLevel - 1) / 5 + 1;
        

        int randomLevelIndex = Random.Range(minLevelIndex, maxLevelIndex + 1);
        int randomItemIndex = Random.Range(0, levelItemCount[randomLevelIndex]); // 다음 인덱스 번호를 저장하고 있기 때문에 +1 해주지 않아도 됨

        int count = 0;

        Debug.Log("randomItemIndex : " + randomItemIndex);
        Debug.Log("levelItemCount[randomLevelIndex] : " + levelItemCount[randomLevelIndex]);

        for (int i = 0; i < itemObjects.Length; ++i)
        {
            if (itemObjects[i].itemData.id >= randomLevelIndex * 1000 && itemObjects[i].itemData.id < (randomLevelIndex + 1) * 1000)
            {
                if (randomItemIndex == count) return itemObjects[i];
                else count++;
            }
        }

        return null;
    }

    #endregion Methods
}