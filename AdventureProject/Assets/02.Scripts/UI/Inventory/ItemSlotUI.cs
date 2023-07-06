using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : UI_Base
{
    #region Enum
    enum GameObjects 
    {
        HeaderArea,
        ButtonArea,
        ContentArea
    }
    enum Images 
    {
        HeaderImage
    }
    enum Buttons 
    {
        QuitButton,
        // SortButton
        // TrimButton,
        // EtcButton
    }
    enum Texts { }
    enum Toggles { }
    #endregion
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
    [SerializeField] private ItemInfo _itemInfo;

    private Image _slotSprite;
    #endregion

    #region Properties
    public bool HasItem() { return _option.itemSprite.sprite != null; }
    public int Index { get { return _itemInfo.Index; } }
    public Image ItemImage { get { return _option.itemSprite; } }
    public Sprite ItemSprite { get { return _option.itemSprite.sprite; } }
    #endregion

    #region Unity Events
    private void Awake()
    {
        Init();
    }
    #endregion

    #region Public Methods
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _option.amountText.gameObject.SetActive(false);
        _option.itemSprite.gameObject.SetActive(false);

        _option.itemSprite.color = _option.slotEmptyColor;

        BindObjects();

        return true;
    }

    public void SetItemSprite(Sprite sprite)
    {
        if (!sprite)
        {
            _option.amountText.gameObject.SetActive(false);
            _option.itemSprite.sprite = sprite;
            _option.itemSprite.color = _option.slotEmptyColor;
            return;
        }

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

    #region Private Methods
    private void BindObjects()
    {
        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindToggle(typeof(Toggles));
    }
    #endregion
}
