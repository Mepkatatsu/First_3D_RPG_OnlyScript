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

        // ������ �̸�/���� ����
        Color color = new();
        if (item.ItemRank == 0) ColorUtility.TryParseHtmlString("#AFA98A", out color);
        else if (item.ItemRank == 1) ColorUtility.TryParseHtmlString("#95C031", out color);
        else if (item.ItemRank == 2) ColorUtility.TryParseHtmlString("#31CAFF", out color);
        else if (item.ItemRank == 3) ColorUtility.TryParseHtmlString("#FFD12E", out color);
        else if (item.ItemRank == 4) ColorUtility.TryParseHtmlString("#FF7100", out color);

        ItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        ItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text = item.name;

        // ������ ��� ����
        itemRankImage.GetComponent<Image>().sprite = UIManager.Instance.itemRankImage[item.ItemRank];


        // ������ ���� �̹��� ����
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


        // ������ ���� �ؽ�Ʈ ����
        if ((int)item.Type < 5) ItemTexts.transform.GetChild(1).GetComponent<TMP_Text>().text = "���� : <color=#CD9A01>���</color>";
        else ItemTexts.transform.GetChild(1).GetComponent<TMP_Text>().text = "���� : <color=#CD9A01>�׼�����</color>";

        for (int i = 0; i < 3; ++i)
        {
            scrollViewContent.transform.GetChild(3 + i).gameObject.SetActive(false);
        }

        // ��ȯ �Ұ�/���� ������ UI ����
        if (item.IsTradable) scrollViewContent.transform.GetChild(2).gameObject.SetActive(false);
        else scrollViewContent.transform.GetChild(2).gameObject.SetActive(true);


        // ���� ���� �ؽ�Ʈ ����
        int equipableLevel = itemDatabase.FindItem(item.id).equipableLevel;
        if (equipableLevel > PlayerCharacterController.Instance.Level)
        {
            scrollViewContent.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "���� ���� : <color=#FF0000>" + equipableLevel;
        }
        else
        {
            scrollViewContent.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "���� ���� : <color=#FFFFFF>" + equipableLevel;
        }


        // ������ ���� �ɷ� ǥ��
        for (int i = 0; i < item.stats.Length; ++i)
        {
            GameObject go = scrollViewContent.transform.GetChild(3 + i).gameObject;
            go.SetActive(true);

            // �ɷ� �̸� ����
            if (item.stats[i].type == AttributeType.PhysicalAttack || item.stats[i].type == AttributeType.PhysicalAttackPercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "���� ���ݷ�";
            }
            else if (item.stats[i].type == AttributeType.MagicalAttack || item.stats[i].type == AttributeType.MagicalAttackPercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "���� ���ݷ�";
            }
            else if (item.stats[i].type == AttributeType.PhysicalDefense || item.stats[i].type == AttributeType.PhysicalDefensePercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "���� ����";
            }
            else if (item.stats[i].type == AttributeType.MagicalDefense || item.stats[i].type == AttributeType.MagicalDefensePercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "���� ����";
            }
            else if (item.stats[i].type == AttributeType.PhysicalAttributeResist)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "���� �Ӽ� ����";
            }
            else if (item.stats[i].type == AttributeType.IceAttributeResist)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "���� �Ӽ� ����";
            }
            else if (item.stats[i].type == AttributeType.FireAttributeResist)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "�� �Ӽ� ����";
            }
            else if (item.stats[i].type == AttributeType.PoisonAttributeResist)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "�� �Ӽ� ����";
            }
            else if (item.stats[i].type == AttributeType.LightAttributeResist)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "�� �Ӽ� ����";
            }
            else if (item.stats[i].type == AttributeType.MaxHPPercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "�ִ� ü��";
            }
            else if (item.stats[i].type == AttributeType.MaxMPPercent)
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "�ִ� ����";
            }
            else
            {
                go.transform.GetChild(1).GetComponent<TMP_Text>().text = "???";
            }

            // ��ġ ����
            if (item.stats[i].type == AttributeType.PhysicalAttack || item.stats[i].type == AttributeType.MagicalAttack ||
                item.stats[i].type == AttributeType.PhysicalDefense || item.stats[i].type == AttributeType.MagicalDefense)
            {
                go.transform.GetChild(2).GetComponent<TMP_Text>().text = item.stats[i].GetFinalValue().ToString();
            }
            else
            {
                go.transform.GetChild(2).GetComponent<TMP_Text>().text = item.stats[i].GetFinalValue().ToString() + "%";
            }

            // �̰��� �������̶�� ��ġ ???�� ����
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
