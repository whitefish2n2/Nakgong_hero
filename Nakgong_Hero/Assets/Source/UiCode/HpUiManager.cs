using Source.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.UiCode
{
    public class HpUiManager : MonoBehaviour
    {
        public static HpUiManager instance;
        [SerializeField] private GameObject hpBar;
        private Image _hpBarImage;
        [SerializeField] private GameObject hpText;
        private TextMeshProUGUI _hpTextTmp;

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            _hpBarImage = hpBar.GetComponent<Image>();
            _hpTextTmp = hpText.GetComponent<TextMeshProUGUI>();
        }
        public void UpdateHpBar()
        {
            _hpBarImage.fillAmount = InvManager.Instance.hp / InvManager.Instance.maxHp;
            _hpTextTmp.text = InvManager.Instance.hp + "/" + InvManager.Instance.maxHp;
        }
    }
}
