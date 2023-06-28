using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    #region Variables
    [System.Serializable]
    public class Option
    {
        public Color slotEmptyColor;
        public Color slotFilledColor;

        public Image itemSprite;

        public TextMeshProUGUI amountText;
    }
    [SerializeField] private Option _option;

    private Image _slotSprite;
    #endregion

    #region Properties
    #endregion

    #region Unity Events
    private void Awake()
    {
        _option.amountText.gameObject.SetActive(false);
        _option.itemSprite.gameObject.SetActive(false);

        _option.itemSprite.color = _option.slotEmptyColor;
    }
    #endregion

    #region Public Methods
    public void SetItemSprite(Sprite sprite)
    {
        if (!sprite)
        {
            return;
        }

        _option.itemSprite.gameObject.SetActive(true);
        _option.itemSprite.sprite = sprite;
        _option.itemSprite.color = _option.slotFilledColor;
    }

    public void SetTextAmount(int amount)
    {
        if(amount < 0 || amount > 9999) { return; }

        _option.amountText.gameObject.SetActive(true);
        _option.amountText.text = $"{amount}";
    }

    public void Clear()
    {
        _option.itemSprite.gameObject.SetActive(false);
        _option.itemSprite.sprite = null;
        _option.itemSprite.color = _option.slotEmptyColor;

        _option.amountText.gameObject.SetActive(false);
        _option.amountText.text = null;
    }
    #endregion
}
