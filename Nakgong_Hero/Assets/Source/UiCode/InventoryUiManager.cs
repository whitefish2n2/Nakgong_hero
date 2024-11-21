using System.Collections;
using System.Collections.Generic;
using Source.Item;
using Source.PlayerCode;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Source.UiCode
{
    public class InventoryUiManager : MonoBehaviour
    {
        [Header("인벤토리 아이템 UI")] 
        [SerializeField] private int colInterval;
        [SerializeField] private int rowInterval;
        [SerializeField] private int maxInOneRow;
        [SerializeField] private float uiSpeed;
        [SerializeField] private GameObject invItemPrefab;
        [SerializeField] private GameObject itemDescriptionPanel;
        [SerializeField] private RectTransform itemDescriptionPanelRect;
        [SerializeField] private TextMeshProUGUI itemDescriptionBody;
        [SerializeField] private TextMeshProUGUI itemDescriptionName;
        [SerializeField] private Image descriptionItemIcon;
        [SerializeField] private Animator descriptionPanelAnimator;
        [SerializeField] private GameObject itemPanel;
        [SerializeField] private GameObject itemNamePanel;
        [SerializeField] private List<GameObject> itemsList = new();
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private Animator panelAnimator;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private List<TextMeshProUGUI> statsText = new();
        private bool _isUiOn;
        private IEnumerator _currentUiSwitchAnim;
        private IEnumerator _descriptActionManager;
        public static InventoryUiManager Instance;
        public ItemInInventory currentHoverItem;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            panelRect = uiPanel.GetComponent<RectTransform>();
            panelRect.localPosition = new Vector3(0,-1500,0);
            _descriptActionManager = DescriptionUiActionManager();
        }

        private void Start()
        {
            ReFreshStats();
            descriptionPanelAnimator.Play("DescriptFadeOut");
        }

        public void SwitchInvUi()
        {
            if (_isUiOn)
            {
                StopCoroutine(_descriptActionManager);
                if (_currentUiSwitchAnim != null)
                {
                    StopCoroutine(_currentUiSwitchAnim);
                }
                _currentUiSwitchAnim = UI_Down();
                
            }
            else
            {
                StartCoroutine(_descriptActionManager);
                if (_currentUiSwitchAnim != null)
                {
                    StopCoroutine(_currentUiSwitchAnim);
                }
                _currentUiSwitchAnim = UI_On();
            }
            StartCoroutine(_currentUiSwitchAnim);
        }

        private void UpdateItems()
        {
            var i = 0;
            foreach (var o in itemsList)
            {
                o.transform.SetParent(itemPanel.GetComponent<RectTransform>());
                o.GetComponent<RectTransform>().localPosition = new Vector2(i % maxInOneRow*colInterval+100,i / maxInOneRow*-rowInterval-100 );
                i++;
            }
        }

        public void AddItem(CommonItem item)
        {
            var o = Instantiate(invItemPrefab, itemPanel.GetComponent<RectTransform>(), true);
            itemsList.Add(o);
            var oItem = o.GetComponent<ItemInInventory>();
            var oImage = o.GetComponent<Image>();
            oItem.Init(item);
            o.GetComponent<RectTransform>().parent = itemPanel.GetComponent<RectTransform>();
            oImage.sprite = item.invIcon;
        }

        public void RemoveItem(ItemInInventory item)
        {
            itemsList.Remove(item.gameObject);
            item.Remove();
            Destroy(item.gameObject);
            UpdateItems();
        }

        public void HoverItem(ItemInInventory item)
        {
            itemDescriptionName.text = item.itemInfo.ItemName;
            itemDescriptionBody.text = item.itemInfo.Discription;
            descriptionItemIcon.sprite = item.itemInfo.InvSprite;
            currentHoverItem = item;
            descriptionPanelAnimator.Play("DescriptFadeIn");
        }

        public void DisHoverItem()
        {
            descriptionPanelAnimator.Play("DescriptFadeOut");
            currentHoverItem = null;
        }
        private IEnumerator DescriptionUiActionManager()
        {
            while (true)
            {
                itemDescriptionPanelRect.position = Input.mousePosition;
                itemDescriptionPanelRect.localPosition = new Vector2(math.clamp(itemDescriptionPanelRect.localPosition.x,-960,580), math.clamp(itemDescriptionPanelRect.localPosition.y,-240,260));
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
        private IEnumerator UI_On()
        {
            if (PlayerController.instance.isStop) yield break;
            Time.timeScale = 0f;
            PlayerController.instance.Stop();
            uiPanel.SetActive(true);
            _isUiOn = true;
            while (panelRect.localPosition.y < 0)
            {
                panelRect.position += Vector3.up * (Time.unscaledDeltaTime * uiSpeed);
                yield return null;
            }
            panelRect.localPosition = new Vector3(0,0,0);
            panelAnimator.Play("Umzzil");
        }
        private IEnumerator UI_Down()
        {
            Time.timeScale = 1f;
            PlayerController.instance.DisStop();
            panelAnimator.Play("Umzzil");
            _isUiOn = false;
            yield return new WaitForSecondsRealtime(0.1f);
            while (uiPanel.transform.localPosition.y > -1500)
            {
                panelRect.position -= Vector3.up * (Time.unscaledDeltaTime * uiSpeed);
                yield return null;
            }
            panelRect.localPosition = new Vector3(0,-1500,0);
            uiPanel.SetActive(false);
        }
        private enum StatListIndex
        {
            Gold,
            Hp,
            Atk,
            Def,
            Spd,
            Wgt,
            Luk,
            Jmp,
            Orb,
            Brk
        }
        private void ReFreshStats()
        {
            Debug.Log(statsText[0].text);
            statsText[(int)StatListIndex.Gold].text = ((int)InvManager.instance.gold).ToString();
            statsText[(int)StatListIndex.Hp].text = (int)InvManager.instance.hp + "/" + (int)InvManager.instance.maxHp;
            statsText[(int)StatListIndex.Atk].text = InvManager.instance.GetAttackPower().ToString("F2");
            statsText[(int)StatListIndex.Def].text = InvManager.instance.GetDefense().ToString("F2");
            statsText[(int)StatListIndex.Spd].text = InvManager.instance.speed.ToString("F2");
            statsText[(int)StatListIndex.Wgt].text = InvManager.instance.airBonePower.ToString("F2");
            statsText[(int)StatListIndex.Luk].text = InvManager.instance.luck.ToString("F2");
            statsText[(int)StatListIndex.Jmp].text = InvManager.instance.jumpPower.ToString("F2");
            statsText[(int)StatListIndex.Orb].text = InvManager.instance.orb.ToString("F2");
            statsText[(int)StatListIndex.Brk].text = InvManager.instance.stansBreak.ToString("F2");
        }
    }
}
