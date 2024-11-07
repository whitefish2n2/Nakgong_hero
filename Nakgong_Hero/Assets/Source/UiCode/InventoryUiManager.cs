using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Source.Item;
using Source.PlayerCode;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private List<GameObject> itemsList = new();
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private Animator panelAnimator;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private List<TextMeshProUGUI> statsText = new();
        private bool _isUiOn;
        private IEnumerator _currentUiSwitchAnim;
        private IEnumerator _mouseChecker;
        public static InventoryUiManager Instance;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            panelRect = uiPanel.GetComponent<RectTransform>();
            panelRect.localPosition = new Vector3(0,-1500,0);
            itemDescriptionPanelRect = itemPanel.GetComponent<RectTransform>();
            _mouseChecker = DescriptionUiPositionFixer();
        }

        private void Start()
        {
            ReFreshStats();
            uiPanel.SetActive(false);
            
        }

        public void SwitchInvUi()
        {
            if (_isUiOn)
            {
                StopCoroutine(_mouseChecker);
                _isUiOn = false;
                if (_currentUiSwitchAnim != null)
                {
                    StopCoroutine(_currentUiSwitchAnim);
                }
                _currentUiSwitchAnim = UI_Down();
            }
            else
            {
                StartCoroutine(_mouseChecker);
                _isUiOn = true;
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
            var oItem = o.GetComponent<CommonItem>();
            oItem.Init(item);
            o.GetComponent<RectTransform>().localPosition = new Vector2(
                itemsList.Count % maxInOneRow * colInterval + 100, itemsList.Count / maxInOneRow * -rowInterval - 100);
        }

        public void RemoveItem(CommonItem item)
        {
            throw new NotImplementedException();
        }

        public void HoverItem(CommonItem item)
        {
            itemDescriptionName.text = item.ItemName;
            itemDescriptionBody.text = item.Discription;
            descriptionItemIcon.sprite = item.InvSprite;
            descriptionPanelAnimator.Play("DescriptFadeIn");
        }

        public void DisHoverItem()
        {
            descriptionPanelAnimator.Play("DescriptFadeOut");
        }
        private IEnumerator DescriptionUiPositionFixer()
        {
            while (true)
            {
                itemDescriptionPanelRect.position = Input.mousePosition;
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
        private IEnumerator UI_On()
        {
            if (PlayerController.Instance.isStop) yield break;
            Time.timeScale = 0f;
            PlayerController.Instance.Stop();
            uiPanel.SetActive(true);
            while (panelRect.localPosition.y < 0)
            {
                panelRect.localPosition += Vector3.up * (Time.unscaledDeltaTime * uiSpeed);
                yield return null;
            }
            panelRect.localPosition = new Vector3(0,0,0);
            panelAnimator.Play("Umzzil");
        }
        private IEnumerator UI_Down()
        {
            Time.timeScale = 1f;
            PlayerController.Instance.DisStop();
            panelAnimator.Play("Umzzil");
            yield return new WaitForSecondsRealtime(0.1f);
            while (uiPanel.transform.localPosition.y > -1500)
            {
                panelRect.localPosition -= Vector3.up * (Time.unscaledDeltaTime * uiSpeed);
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
            statsText[(int)StatListIndex.Gold].text = ((int)InvManager.Instance.gold).ToString();
            statsText[(int)StatListIndex.Hp].text = (int)InvManager.Instance.hp + "/" + (int)InvManager.Instance.maxHp;
            statsText[(int)StatListIndex.Atk].text = InvManager.Instance.attackPower.ToString("F2");
            statsText[(int)StatListIndex.Def].text = InvManager.Instance.defense.ToString("F2");
            statsText[(int)StatListIndex.Spd].text = InvManager.Instance.speed.ToString("F2");
            statsText[(int)StatListIndex.Wgt].text = InvManager.Instance.airBonePower.ToString("F2");
            statsText[(int)StatListIndex.Luk].text = InvManager.Instance.luck.ToString("F2");
            statsText[(int)StatListIndex.Jmp].text = InvManager.Instance.jumpPower.ToString("F2");
            statsText[(int)StatListIndex.Orb].text = InvManager.Instance.orb.ToString("F2");
            statsText[(int)StatListIndex.Brk].text = InvManager.Instance.stansBreak.ToString("F2");
        }
    }
}
