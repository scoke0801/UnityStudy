using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    #region Fields
    [System.Serializable]
    public class Options
    {
        [Range(0, 10)] public int horizontalSlotCount = 8;
        [Range(0, 10)] public int verticalSlotCount = 8;
        [Range(32, 64)] public float slotSize = 64f;

        public float slotMargin = 10f;
        public float padding = 20f;
    }
    [SerializeField] private Options _option;

    [System.Serializable]
    public class ConnectedObjects
    {
        public RectTransform contentArea;
        public GameObject slotUIPrefab;
    }
    [SerializeField] ConnectedObjects _connectedObject;

    private List<ItemSlotUI> _slotUIList;
    #endregion

    #region Unity Events
    private void Awake()
    {
        InitSlots();
    }

    private void Update()
    {
        
    }
    #endregion

    #region Init
    void InitSlots()
    {
        _connectedObject.slotUIPrefab.TryGetComponent(out RectTransform slotRect);
        slotRect.sizeDelta = new Vector2(_option.slotSize, _option.slotSize);

        _connectedObject.slotUIPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if( itemSlot == null)
        {
            _connectedObject.slotUIPrefab.AddComponent<ItemSlotUI>();
        }

        _connectedObject.slotUIPrefab.SetActive(false);
      
        Vector2 contentAreaSize = new Vector2(_connectedObject.contentArea.rect.width, _connectedObject.contentArea.rect.height);

        Debug.Log($"ContentSizeDelta: {_connectedObject.contentArea.sizeDelta}");
        Debug.Log($"ContentSize: {contentAreaSize}");

        Vector2 beginPos = new Vector2(-contentAreaSize.x * 0.5f + _option.padding, contentAreaSize.y * 0.5f -_option.padding);
        Vector2 curPos = beginPos;

        _slotUIList = new List<ItemSlotUI>(_option.verticalSlotCount * _option.horizontalSlotCount);
    
        for(int y = 0; y < _option.verticalSlotCount; ++y)
        {
            for(int x = 0; x < _option.horizontalSlotCount; ++x)
            {
                int slotIndex = (y * _option.horizontalSlotCount) + x;

                RectTransform slot = CloneSlot();
                slot.anchoredPosition = curPos;
                //slot.position = curPos;
                slot.gameObject.SetActive(true);
                slot.gameObject.name = $"Item SLot [{slotIndex}]";
                slot.localScale = new Vector3(1f, 1f, 1f);

                curPos.x += (_option.slotMargin + _option.slotSize);
            }

            curPos.x = beginPos.x;
            curPos.y -= (_option.slotMargin + _option.slotSize);
        }

        if(_connectedObject.slotUIPrefab.scene.rootCount != 0)
        {
            Destroy(_connectedObject.slotUIPrefab);
        }
    }

    private RectTransform CloneSlot()
    {
        GameObject slot = Instantiate(_connectedObject.slotUIPrefab);
        RectTransform rt = slot.GetComponent<RectTransform>();
        rt.SetParent(_connectedObject.contentArea);

        return rt;
    }
    #endregion
}
