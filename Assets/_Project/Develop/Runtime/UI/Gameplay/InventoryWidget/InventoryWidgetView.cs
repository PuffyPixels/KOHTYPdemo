using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.UI.Core;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.InventoryWidget
{
    public class InventoryWidgetView : MonoBehaviour, IShowableView
    {
        [SerializeField]
        private InventorySlot _originSlot;

        private Transform _slotsContainer;
        private List<InventorySlot> _slots;
        private int _selectedSlot = -1;

        private void Awake()
        {
            _slotsContainer = _originSlot.transform.parent;
        }


        public void SetCapacity(int capacity)
        {
            capacity = capacity < 1 ? 1 : capacity;
            _slots = new(capacity) { _originSlot };

            for (int i = _slotsContainer.childCount; i < capacity; i++)
            {
                _slots.Add(Instantiate(_originSlot, _slotsContainer));
            }

            for (int i = capacity; i < _slotsContainer.childCount; i++)
            {
                Destroy(_slotsContainer.GetChild(i).gameObject);
            }
        }

        public void OnItemsChanged(IReadOnlyCollection<InventoryItem> items)
        {
            for (int i = 0; i < items.Count && i < _slots.Count; i++)
            {
                _slots[i].SetItem(items.ElementAt(i));
            }

            for (int i = items.Count; i < _slots.Count; i++)
            {
                _slots[i].Clear();
            }
        }

        public void OnSlotSelected(int slotId)
        {
            ResetSelection(_selectedSlot);
            _selectedSlot = -1;

            if (slotId >= 0 && slotId < _slots.Count)
            {
                _selectedSlot = slotId;
                Select(_selectedSlot);
            }
        }

        public Tween Hide()
        {
            _originSlot.transform.parent.gameObject.SetActive(false);
            return DOTween.Sequence();
        }

        public Tween Show()
        {
            _originSlot.transform.parent.gameObject.SetActive(true);
            return DOTween.Sequence();
        }

        private void Select(int slotId)
        {
            if (slotId < 0 || slotId >= _slots.Count)
            {
                return;
            }

            _slots[slotId].Select();
        }

        private void ResetSelection(int slotId)
        {
            if (slotId < 0 || slotId >= _slots.Count)
            {
                return;
            }

            _slots[slotId].Deselect();
        }
    }
}