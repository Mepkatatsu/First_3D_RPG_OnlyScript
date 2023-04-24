using SingletonPattern;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStatusUI : MonoBehaviour
{
    #region Variables

    public GameObject itemSlot;
    public GameObject ItemTexts;
    public GameObject scrollViewContent;
    public GameObject abilityTextPrefab;
    public GameObject itemRankImage;
    public GameObject equipButtonParent;
    public ItemDatabaseObject itemDatabase;

    public ItemData currentItemData;
    public int currentItemIndex;

    #endregion Variables

    #region Methods

    public void UpdateItemStatus(ItemData item, int amount, int index)
    {
        currentItemData = item;
        currentItemIndex = index;

        // 아이템 이름/색상 변경
        Color color = new();
        if (item.ItemRank == 0) ColorUtility.TryParseHtmlString("#AFA98A", out color);
        else if (item.ItemRank == 1) ColorUtility.TryParseHtmlString("#95C031", out color);
        else if (item.ItemRank == 2) ColorUtility.TryParseHtmlString("#31CAFF", out color);
        else if (item.ItemRank == 3) ColorUtility.TryParseHtmlString("#FFD12E", out color);
        else if (item.ItemRank == 4) ColorUtility.TryParseHtmlString("#FF7100", out color);

        ItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        ItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text = item.name;

        // 아이템 등급 변경
        itemRankImage.GetComponent<Image>().sprite = UIManager.Instance.itemRankImage[item.ItemRank];


        // 아이템 슬롯 이미지 변경
        itemSlot.transform.GetChild(0).GetComponent<Image>().sprite = itemDatabase.FindItem(item.id).icon;
        itemSlot.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);

        if (itemSlot.transform.GetChild(1).GetComponent<Image>() != null)
        {
            itemSlot.transform.GetChild(1).GetComponent<Image>().sprite = UIManager.Instance.itemRankImage[item.ItemRank];
            itemSlot.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }

        if (item.IsStarItem && !item.IsEvaluated)
        {
            itemSlot.transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else itemSlot.transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0);

        if (item.IsStarItem && item.IsEvaluated)
        {
            itemSlot.transform.GetChild(3).GetComponent<Image>().sprite = UIManager.Instance.starRankImage[item.StarRank];
            itemSlot.transform.GetChild(3).GetComponent<Image>().color = Color.white;
        }
        else
        {
            itemSlot.transform.GetChild(3).GetComponent<Image>().color = Color.clear;
        }

        if (itemSlot.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            itemSlot.GetComponentInChildren<TextMeshProUGUI>().text = amount == 1 ? string.Empty : amount.ToString("n0");
        }


        // 아이템 종류 텍스트 변경
        if ((int)item.Type < 5) ItemTexts.transform.GetChild(1).GetComponent<TMP_Text>().text = "종류 : <color=#CD9A01>장비</color>";
        else ItemTexts.transform.GetChild(1).GetComponent<TMP_Text>().text = "종류 : <color=#CD9A01>액세서리</color>";

        for (int i = 0; i < 3; ++i)
        {
            scrollViewContent.transform.GetChild(3 + i).gameObject.SetActive(false);
        }

        // 교환 불가/가능 아이템 UI 변경
        if (item.IsTradable) scrollViewContent.transform.GetChild(2).gameObject.SetActive(false);
        else scrollViewContent.transform.GetChild(2).gameObject.SetActive(true);


        // 레벨 제한 텍스트 변경
        int equipableLevel = itemDatabase.FindItem(item.id).equipableLevel;
        if (equipableLevel > PlayerCharacterController.Instance.Level)
        {
            scrollViewContent.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "레벨 제한 : <color=#FF0000>" + equipableLevel;
        }
        else
        {
            scrollViewContent.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "레벨 제한 : <color=#FFFFFF>" + equipableLevel;
        }


        // 아이템 보유 능력 표시
        for (int i = 0; i < item.stats.Length; ++i)
        {
            GameObject go = scrollViewContent.transform.GetChild(3 + i).gameObject;
            go.SetActive(true);

            // 능력 이름 변경
            if (item.stats[i].type == AttributeType.PhysicalAttack || item.stats[i].type == AttributeType.PhysicalAttackPercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "물리 공격력";
            }
            else if (item.stats[i].type == AttributeType.MagicalAttack || item.stats[i].type == AttributeType.MagicalAttackPercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "마법 공격력";
            }
            else if (item.stats[i].type == AttributeType.PhysicalDefense || item.stats[i].type == AttributeType.PhysicalDefensePercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "물리 방어력";
            }
            else if (item.stats[i].type == AttributeType.MagicalDefense || item.stats[i].type == AttributeType.MagicalDefensePercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "마법 방어력";
            }
            else if (item.stats[i].type == AttributeType.PhysicalAttributeResist)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "물리 속성 저항";
            }
            else if (item.stats[i].type == AttributeType.IceAttributeResist)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "얼음 속성 저항";
            }
            else if (item.stats[i].type == AttributeType.FireAttributeResist)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "불 속성 저항";
            }
            else if (item.stats[i].type == AttributeType.PoisonAttributeResist)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "독 속성 저항";
            }
            else if (item.stats[i].type == AttributeType.LightAttributeResist)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "빛 속성 저항";
            }
            else if (item.stats[i].type == AttributeType.MaxHPPercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "최대 체력";
            }
            else if (item.stats[i].type == AttributeType.MaxMPPercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "최대 마나";
            }
            else
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "???";
            }

            // 수치 변경
            if (item.stats[i].type == AttributeType.PhysicalAttack || item.stats[i].type == AttributeType.MagicalAttack ||
                item.stats[i].type == AttributeType.PhysicalDefense || item.stats[i].type == AttributeType.MagicalDefense)
            {
                go.transform.GetChild(2).GetComponent<TMP_Text>().text = item.stats[i].GetFinalValue().ToString();
            }
            else
            {
                go.transform.GetChild(2).GetComponent<TMP_Text>().text = item.stats[i].GetFinalValue().ToString() + "%";
            }

            // 미감정 아이템이라면 수치 ???로 변경
            if (item.IsStarItem && !item.IsEvaluated) go.transform.GetChild(2).GetComponent<TMP_Text>().text = "???";
        }

        if (item.IsStarItem && !item.IsEvaluated)
        {
            transform.GetChild(9).gameObject.SetActive(false);
            transform.GetChild(10).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(9).gameObject.SetActive(true);
            transform.GetChild(10).gameObject.SetActive(false);
        }
    }

    #endregion Methods
}
