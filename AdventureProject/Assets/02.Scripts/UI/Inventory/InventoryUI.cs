using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    // Drag ����------------------------------------------------------------------------
    private Vector3 _slotDragStartPoint;
    private Vector3 _cursorDragStartPoint;

    private int _slotSiblingIndex;

    private ItemSlotUI _selectedSlot;

    private GraphicRaycaster _graphicRaycaster;
    private PointerEventData _pointEventData;

    private List<RaycastResult> _raycastResultList;
    //----------------------------------------------------------------------------------
    #endregion

    #region Unity Events
    private void Start()
    {
        Init();
        InitSlots();
    }

    private void Update()
    {
        _pointEventData.position = Input.mousePosition;

        CheckDrag();
        CheckButtonUp();
    }
    #endregion

    #region Init
    private void Init()
    {
        transform.parent.TryGetComponent(out _graphicRaycaster);
        if (!_graphicRaycaster)
        {
            _graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        }

        _pointEventData = new PointerEventData(EventSystem.current);
        _raycastResultList = new List<RaycastResult>(10);
    }

    void InitSlots()
    {
        _connectedObject.slotUIPrefab.TryGetComponent(out RectTransform slotRect);
        slotRect.sizeDelta = new Vector2(_option.slotSize, _option.slotSize);

        _connectedObject.slotUIPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if (itemSlot == null)
        {
            _connectedObject.slotUIPrefab.AddComponent<ItemSlotUI>();
        }

        _connectedObject.slotUIPrefab.SetActive(false);

        Vector2 contentAreaSize = new Vector2(_connectedObject.contentArea.rect.width, _connectedObject.contentArea.rect.height);
        Vector2 beginPos = new Vector2(-contentAreaSize.x * 0.5f + _option.padding, contentAreaSize.y * 0.5f - _option.padding);
        Vector2 curPos = beginPos;

        _slotUIList = new List<ItemSlotUI>(_option.verticalSlotCount * _option.horizontalSlotCount);

        for (int y = 0; y < _option.verticalSlotCount; ++y)
        {
            for (int x = 0; x < _option.horizontalSlotCount; ++x)
            {
                int slotIndex = (y * _option.horizontalSlotCount) + x;

                RectTransform slot = CloneSlot();
                slot.anchoredPosition = curPos;
                //slot.position = curPos;
                slot.gameObject.SetActive(true);
                slot.gameObject.name = $"Item SLot [{slotIndex}]";
                slot.localScale = new Vector3(1f, 1f, 1f);

                ItemSlotUI itemSlotUI = slot.GetComponent<ItemSlotUI>();
                if (Random.Range(0, 100) > 50)
                {
                    itemSlotUI.SetItemSprite(TestSpriteManager.Instante.GetRandomSprite());
                    itemSlotUI.SetTextAmount(Random.Range(1, 9999));
                }
                _slotUIList.Add(itemSlotUI);

                curPos.x += (_option.slotMargin + _option.slotSize);
            }

            curPos.x = beginPos.x;
            curPos.y -= (_option.slotMargin + _option.slotSize);
        }

        if (_connectedObject.slotUIPrefab.scene.rootCount != 0)
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

    #region Public Methods
    public void CheckButtonDown()
    {
        if (!Input.GetMouseButtonDown(Define.Input.LEFT_CLICK)) { return; }

        ItemSlotUI slotUI = RaycastAndGetFirstComponent<ItemSlotUI>();
        if (!slotUI) { return; }
        _selectedSlot = slotUI;
        _slotDragStartPoint = slotUI.transform.position;
        _cursorDragStartPoint = Input.mousePosition;

        _slotSiblingIndex = slotUI.transform.GetSiblingIndex();
        slotUI.transform.SetAsLastSibling();
    }
    #endregion

    #region Private Methods
    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _raycastResultList.Clear();

        _graphicRaycaster.Raycast(_pointEventData, _raycastResultList);

        if(_raycastResultList.Count == 0)
        {
            return null;
        }

        return _raycastResultList[0].gameObject.GetComponent<T>();
    }

    private void CheckDrag() 
    {
        if (!Input.GetMouseButton(Define.Input.LEFT_CLICK)) { return; }
        if (!_selectedSlot) { return; }

        _selectedSlot.transform.position =
            _slotDragStartPoint + (Input.mousePosition - _cursorDragStartPoint);
    }

    private void CheckButtonUp()
    {
        if (!Input.GetMouseButtonUp(Define.Input.LEFT_CLICK)) { return; }
        if (!_selectedSlot) { return; }

        _selectedSlot.transform.position = _slotDragStartPoint;
        _selectedSlot.transform.SetSiblingIndex(_slotSiblingIndex);
        _selectedSlot = null;

        EndDrag();
    }

    private void EndDrag()
    {

    }
    #endregion


    #region EDITOR
#if UNITY_EDITOR
    [SerializeField] private bool __showPreview = false;

    [Range(0.01f, 1f)]
    [SerializeField] private float __previewAlpha = 0.1f;

    private List<GameObject> __previewSlotGoList = new List<GameObject>();
    private int __prevSlotCountPerLine;
    private int __prevSlotLineCount;
    private float __prevSlotSize;
    private float __prevSlotMargin;
    private float __prevContentPadding;
    private float __prevAlpha;
    private bool __prevShow = false;

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (__showPreview && !__prevShow)
        {
            CreateSlots();
        }
        __prevShow = __showPreview;

        if (Unavailable())
        {
            ClearAll();
            return;
        }
        if (CountChanged())
        {
            ClearAll();
            CreateSlots();
            __prevSlotCountPerLine = _option.horizontalSlotCount;
            __prevSlotLineCount = _option.verticalSlotCount;
        }
        if (ValueChanged())
        {
            DrawGrid();
            __prevSlotSize = _option.slotSize;
            __prevSlotMargin = _option.slotMargin;
            __prevContentPadding = _option.padding;
        }
        if (AlphaChanged())
        {
            SetImageAlpha();
            __prevAlpha = __previewAlpha;
        }

        bool Unavailable()
        {
            return !__showPreview ||
                    _option.horizontalSlotCount < 1 ||
                    _option.verticalSlotCount < 1 ||
                    _option.slotSize <= 0f ||
                    _connectedObject.contentArea == null ||
                    _connectedObject.slotUIPrefab == null;
        }
        bool CountChanged()
        {
            return _option.horizontalSlotCount != __prevSlotCountPerLine ||
                   _option.verticalSlotCount != __prevSlotLineCount;
        }
        bool ValueChanged()
        {
            return _option.slotSize != __prevSlotSize ||
                   _option.slotMargin != __prevSlotMargin ||
                   _option.padding != __prevContentPadding;
        }
        bool AlphaChanged()
        {
            return __previewAlpha != __prevAlpha;
        }
        void ClearAll()
        {
            foreach (var go in __previewSlotGoList)
            {
                Destroyer.Destroy(go);
            }
            __previewSlotGoList.Clear();
        }
        void CreateSlots()
        {
            int count = _option.horizontalSlotCount * _option.verticalSlotCount;
            __previewSlotGoList.Capacity = count;

            for (int i = 0; i < count; i++)
            {
                GameObject slotGo = Instantiate(_connectedObject.slotUIPrefab);
                slotGo.transform.SetParent(_connectedObject.contentArea);
                slotGo.SetActive(true);
                slotGo.AddComponent<PreviewItemSlot>();

                slotGo.transform.localScale = Vector3.one;

                HideGameObject(slotGo);

                __previewSlotGoList.Add(slotGo);
            }

            DrawGrid();
            SetImageAlpha();
        }
        void DrawGrid()
        {
            Vector2 contentAreaSize = new Vector2(_connectedObject.contentArea.rect.width, _connectedObject.contentArea.rect.height);
            Vector2 beginPos = new Vector2(-contentAreaSize.x * 0.5f + _option.padding, contentAreaSize.y * 0.5f - _option.padding);
            Vector2 curPos = beginPos;

            // Draw Slots
            int index = 0;
            for (int j = 0; j < _option.verticalSlotCount; j++)
            {
                for (int i = 0; i < _option.horizontalSlotCount; i++)
                {
                    GameObject slotGo = __previewSlotGoList[index++];
                    RectTransform slotRT = slotGo.GetComponent<RectTransform>();

                    slotRT.anchoredPosition = curPos;
                    slotRT.sizeDelta = new Vector2(_option.slotSize, _option.slotSize);
                    __previewSlotGoList.Add(slotGo);

                    // Next X
                    curPos.x += (_option.slotMargin + _option.slotSize);
                }

                // Next Line
                curPos.x = beginPos.x;
                curPos.y -= (_option.slotMargin + _option.slotSize);
            }
        }
        void HideGameObject(GameObject go)
        {
            go.hideFlags = HideFlags.HideAndDontSave;

            Transform tr = go.transform;
            for (int i = 0; i < tr.childCount; i++)
            {
                tr.GetChild(i).gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
        }
        void SetImageAlpha()
        {
            foreach (var go in __previewSlotGoList)
            {
                var images = go.GetComponentsInChildren<Image>();
                foreach (var img in images)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, __previewAlpha);
                    var outline = img.GetComponent<Outline>();
                    if (outline)
                        outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, __previewAlpha);
                }
            }
        }
    }

    private class PreviewItemSlot : MonoBehaviour { }

    [UnityEditor.InitializeOnLoad]
    private static class Destroyer
    {
        private static Queue<GameObject> targetQueue = new Queue<GameObject>();

        static Destroyer()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                for (int i = 0; targetQueue.Count > 0 && i < 100000; i++)
                {
                    var next = targetQueue.Dequeue();
                    DestroyImmediate(next);
                }
            };
        }
        public static void Destroy(GameObject go) => targetQueue.Enqueue(go);
    }
#endif
    #endregion
}
