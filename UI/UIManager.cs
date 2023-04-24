using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SingletonPattern
{
    public class UIManager : Singleton<UIManager>
    {
        #region Variables

        public ItemStatusUI itemStatusUI;
        public ItemDatabaseObject itemDatabase;
        public DynamicInventoryUI dynamicInventoryUI;
        public InventoryObject playerInventory;
        public InventoryObject playerEquipment;
        public StatsObject playerStats;
        public GameObject canvas;
        public GameObject damageTextParent;
        public GameObject damageTextPrefab;
        public GameObject inventoryUI;
        public GameObject playerStatusUI;
        public GameObject enemyStatusUI;
        public new Camera camera;
        public Sprite[] itemRankImage;
        public Sprite[] starRankImage;

        private AudioManager _audioManager;
        private GameObject _target;

        [HideInInspector] public bool isOpenInventory = false;

        #endregion Variables

        #region Unity Methods

        private void Start()
        {
            if (_audioManager == null) _audioManager = AudioManager.Instance;

            inventoryUI.SetActive(false);

            playerStats.OnChangedStats.Invoke(playerStats);
        }

        private void OnEnable()
        {
            playerStats.OnChangedStats += OnChangedHPMP;
        }

        private void OnDisable()
        {
            playerStats.OnChangedStats -= OnChangedHPMP;
        }

        #endregion Unity Methods

        #region Methods

        public void UpdateEnemyUI(GameObject target)
        {
            if (target == null)
            {
                enemyStatusUI.SetActive(false);
                return;
            }

            _target = target;

            EnemyController_Melee enemyController = target.GetComponent<EnemyController_Melee>();

            enemyStatusUI.SetActive(true);
            // 레벨, 이름 업데이트
            enemyStatusUI.transform.GetChild(1).GetComponent<TMP_Text>().text = "Lv." + enemyController.EnemyLevel + " " + enemyController.enemyName;

            // 체력바 업데이트
            enemyStatusUI.transform.GetChild(2).GetChild(1).GetComponent<Image>().fillAmount = enemyController.currentHP / enemyController.maxHP;
            enemyStatusUI.transform.GetChild(2).GetChild(2).GetComponent<TMP_Text>().text = string.Format("{0:#,###}", enemyController.currentHP) + " / " + string.Format("{0:#,###}", enemyController.maxHP);
            if (enemyController.currentHP <= 0) enemyStatusUI.transform.GetChild(2).GetChild(2).GetComponent<TMP_Text>().text = 0 + " / " + string.Format("{0:#,###}", enemyController.maxHP);

            // 아이콘 업데이트
            if (enemyController.enemyIcon != null) enemyStatusUI.transform.GetChild(3).GetChild(1).GetComponent<Image>().sprite = enemyController.enemyIcon;
            enemyStatusUI.transform.GetChild(3).GetChild(2).GetComponent<TMP_Text>().text = enemyController.EnemyLevel.ToString();
        }

        public void ResetEnemyUI()
        {
            StartCoroutine(ResetEnemyUICoroutine());
        }

        public IEnumerator ResetEnemyUICoroutine()
        {
            GameObject resetTarget = _target;

            yield return new WaitForSeconds(2);

            if (resetTarget != _target) yield break;

            OutlineManager.Instance.DisableOutline();
            enemyStatusUI.SetActive(false);
            PlayerCharacterController.Instance.target = null;
            _target = null;
        }

        public void ShowEquipButton(bool isAvailable)
        {
            itemStatusUI.equipButtonParent.transform.GetChild(0).gameObject.SetActive(isAvailable);
            itemStatusUI.equipButtonParent.transform.GetChild(1).gameObject.SetActive(!isAvailable);
        }

        public void ShowDamageText(GameObject target, int damage)
        {
            if (target == null) return;

            GameObject damageTextGO = Instantiate(damageTextPrefab, camera.WorldToScreenPoint(target.transform.position), Quaternion.identity, damageTextParent.transform);
            DamageText damageText = damageTextGO.GetComponent<DamageText>();

            int randNum = UnityEngine.Random.Range(30, 60);
            damageTextGO.GetComponent<RectTransform>().position = new Vector2(damageTextGO.GetComponent<RectTransform>().position.x + randNum, damageTextGO.GetComponent<RectTransform>().position.y + randNum);

            damageText.Damage = damage;
        }

        private void OnChangedHPMP(StatsObject statsObject)
        {
            playerStatusUI.transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = statsObject.HPPercentage;
            playerStatusUI.transform.GetChild(1).GetChild(1).GetComponent<Image>().fillAmount = statsObject.MPPercentage;

            playerStatusUI.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = string.Format("{0:#,###}", statsObject.CurrentHP) + " / " + string.Format("{0:#,###}", statsObject.MaxHP);
            playerStatusUI.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = string.Format("{0:#,###}", statsObject.CurrentMP) + " / " + string.Format("{0:#,###}", statsObject.MaxMP);

            if (statsObject.CurrentHP == 0) playerStatusUI.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text  = 0 + " / " + string.Format("{0:#,###}", statsObject.MaxHP);
            if (statsObject.CurrentMP == 0) playerStatusUI.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text  = 0 + " / " + string.Format("{0:#,###}", statsObject.MaxMP);
        }

        public void OnClickAttackButton(int buttonIndex)
        {
            PlayerCharacterController.Instance.ExecuteAttackButton(buttonIndex);
            AudioManager.Instance.PlaySFX("Click");
        }

        public void OnClickOpenInventoryButton()
        {
            if (!inventoryUI.activeSelf)
            {
                isOpenInventory = true;
                inventoryUI.SetActive(true);
            }
            else if (inventoryUI.activeSelf)
            {
                isOpenInventory = false;
                inventoryUI.SetActive(false);
            }

            _audioManager.PlaySFX("Click");
        }

        public void OnClickEquipItemButton()
        {
            int slotIndex = 0;

            for (int i = 0; i < playerEquipment.Slots.Count; ++i)
            {
                if (playerEquipment.Slots[i].CanPlaceInSlot(itemStatusUI.currentItemData))
                {
                    playerEquipment.SwapItems(playerEquipment.Slots[i], playerInventory.Slots[itemStatusUI.currentItemIndex]);
                    slotIndex = i;

                    break;
                }
            }

            itemStatusUI.currentItemIndex = slotIndex;
            itemStatusUI.currentItemData = playerEquipment.Slots[slotIndex].itemData;

            ShowEquipButton(false);

            dynamicInventoryUI.ArrangeItem();

            _audioManager.PlaySFX("Equip");
        }

        public void OnClickUnequipItemButton()
        {
            if (playerInventory.EmptySlotCount <= 0) return;

            int emptySlotIndex = playerInventory.GetEmptySlotIndex();

            playerEquipment.SwapItems(playerEquipment.Slots[itemStatusUI.currentItemIndex], playerInventory.GetEmptySlot());

            itemStatusUI.currentItemIndex = emptySlotIndex;
            itemStatusUI.currentItemData = playerInventory.Slots[emptySlotIndex].itemData;

            ShowEquipButton(true);

            _audioManager.PlaySFX("Unequip");
        }

        public void OnClickEquipmentItemButton(int index)
        {
            if (playerEquipment.Slots[index].itemData.id <= -1) return;
            itemStatusUI.gameObject.SetActive(true);
            itemStatusUI.UpdateItemStatus(playerEquipment.Slots[index].itemData, playerEquipment.Slots[index].amount, index);

            ShowEquipButton(false);

            _audioManager.PlaySFX("Click");
        }

        public void OnClickInventoryItemButton(int index)
        {
            if (playerInventory.Slots[index].itemData.id <= -1) return;
            itemStatusUI.gameObject.SetActive(true);
            itemStatusUI.UpdateItemStatus(playerInventory.Slots[index].itemData, playerInventory.Slots[index].amount, index);

            ShowEquipButton(true);

            _audioManager.PlaySFX("Click");
        }

        public void OnClickCloseItemStatusButton()
        {
            itemStatusUI.gameObject.SetActive(false);
            _audioManager.PlaySFX("Click");
        }

        public void OnClickNormalEvaluateButton()
        {
            itemStatusUI.currentItemData.EvaluateItemNormal();
            playerInventory.Slots[itemStatusUI.currentItemIndex].UpdateSlot(itemStatusUI.currentItemData, playerInventory.Slots[itemStatusUI.currentItemIndex].amount);
            itemStatusUI.UpdateItemStatus(playerInventory.Slots[itemStatusUI.currentItemIndex].itemData, playerInventory.Slots[itemStatusUI.currentItemIndex].amount, itemStatusUI.currentItemIndex);

            _audioManager.PlaySFX("Click");
        }
        public void OnClickAdvancedEvaluateButton()
        {
            itemStatusUI.currentItemData.EvaluateItemAdvanced();
            playerInventory.Slots[itemStatusUI.currentItemIndex].UpdateSlot(itemStatusUI.currentItemData, playerInventory.Slots[itemStatusUI.currentItemIndex].amount);
            itemStatusUI.UpdateItemStatus(playerInventory.Slots[itemStatusUI.currentItemIndex].itemData, playerInventory.Slots[itemStatusUI.currentItemIndex].amount, itemStatusUI.currentItemIndex);

            _audioManager.PlaySFX("Click");
        }
    }

    #endregion Methods
}