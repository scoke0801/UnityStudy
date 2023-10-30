using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JHInventory
{
    [RequireComponent(typeof(EventTrigger))]
    public abstract class InventoryUI : MonoBehaviour
    {
        #region Variables
        public InventoryObject _inventoryObject;
        private InventoryObject _previousInventory;

        public Dictionary<GameObject, InventorySlot> _slotUIs = new Dictionary<GameObject, InventorySlot>();
        #endregion

        #region Unity Methods
        private void Awake()
        {
            CreateSlots();

            for (int i = 0; i < _inventoryObject.Slots.Length; ++i)
            {
                _inventoryObject.Slots[i]._parent = _inventoryObject;
                _inventoryObject.Slots[i].OnPostUpdate += OnPostUpdate;
            }

            AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        }
        #endregion

        #region Public Methods
        public abstract void CreateSlots();

        public void OnPostUpdate(InventorySlot slot)
        {
            slot._slotUI.transform.GetChild(0).GetComponent<Image>().sprite = slot.item.id < 0 ? null : slot.ItemObject.icon;
            slot._slotUI.transform.GetChild(0).GetComponent<Image>().color = slot.item.id < 0 ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);
            slot._slotUI.GetComponentInChildren<TextMeshProUGUI>().text = slot.item.id < 0 ? string.Empty : (slot.amount == 1 ? string.Empty : slot.amount.ToString("NO"));
        }
        public void OnEnterInterface(GameObject go)
        {
            MouseData._interfaceMouseIsOver = go.GetComponent<InventoryUI>();
        }
        public void OnExitInterface(GameObject go)
        {
            MouseData._interfaceMouseIsOver = null;
        }

        public void OnEnter(GameObject go)
        {
            MouseData._slotHoveredOver = go;
            MouseData._interfaceMouseIsOver = go.GetComponentInParent<InventoryUI>();
        }

        public void OnExit(GameObject go)
        {
            MouseData._slotHoveredOver = null;
        }

        public void OnStartDrag(GameObject go)
        {
            MouseData._itemBeingDragged = CreateDragImage(go);
        }
        public void OnDrag(GameObject go)
        {
            if(MouseData._itemBeingDragged == null) { return; }

            MouseData._itemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }

        public void OnEndDrag(GameObject go)
        {
            Destroy(MouseData._itemBeingDragged);

            if(MouseData._interfaceMouseIsOver == null)
            {
                _slotUIs[go].RemoveItem();
            }
            else if(MouseData._slotHoveredOver)
            {
                InventorySlot mouseHoverSlotData = MouseData._interfaceMouseIsOver._slotUIs[MouseData._slotHoveredOver];
                _inventoryObject.SwapItems(_slotUIs[go], mouseHoverSlotData);
            }
        }

        public void OnClick(GameObject go, PointerEventData data)
        {
            InventorySlot slot = _slotUIs[go];
            if( slot == null) { return; }

            if(data.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick(slot);
            }
            else if(data.button == PointerEventData.InputButton.Right)
            {
                OnRightClick(slot);
            }
        }
        #endregion

        #region Protected Methods
        protected void AddEvent(GameObject go, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            if(!trigger)
            {
                Debug.LogWarning("NoEventTrigger component found!");
                return;
            }

            EventTrigger.Entry eventTrigger = new EventTrigger.Entry { eventID = type };
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }
        protected virtual void OnRightClick(InventorySlot slot)
        {

        }

        protected virtual void OnLeftClick(InventorySlot slot)
        {

        }
        #endregion

        #region Private Methods
        private GameObject CreateDragImage(GameObject go)
        {
            if (_slotUIs[go].item.id < 0)
            {
                return null;
            }

            GameObject dragImage = new GameObject();

            RectTransform rectTransform = dragImage.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(50, 50);
            dragImage.transform.SetParent(transform.parent);

            Image image = dragImage.AddComponent<Image>();
            image.sprite = _slotUIs[go].ItemObject.icon;
            image.raycastTarget = false;

            dragImage.name = "Drag Image";

            return dragImage;
        }
        #endregion
    }
}
