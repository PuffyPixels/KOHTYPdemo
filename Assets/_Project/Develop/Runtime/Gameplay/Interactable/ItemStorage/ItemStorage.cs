using Assets._Project.Develop.Runtime.Gameplay.Interactable.Item;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using System;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.ItemStorage
{
    public class ItemStorage : Selectable
    {
        public string ItemName => (_item != null && _item.gameObject.activeSelf) ? _item.Name : "";
        public event Action<ItemStorage> ItemChanged;

        private ItemThrower _itemThrower;
        private CollectableItem _item;

        public override string InteractionDescription => _item == null ? Settings.Settings.ITEM_STORAGE_INTERACTION_DESCRIPTION_PUT : 
            Settings.Settings.ITEM_STORAGE_INTERACTION_DESCRIPTION_GET;


        public void Init(ItemThrower itemThrower)
        {
            _itemThrower = itemThrower;
        }

        public override void Select()
        {
            base.Select();

            if (_item == null)
                _itemThrower.ItemThrown += OnItemThrown;
        }

        public override void Deselect()
        {
            base.Deselect();
            _itemThrower.ItemThrown -= OnItemThrown;
        }

        public override void Interact()
        {
            if (_item == null)
                return;

            _item.Interact();
            ItemChanged?.Invoke(this);
            Deselect();
        }

        private void OnDestroy()
        {
            _itemThrower.ItemThrown -= OnItemThrown;
        }

        private void OnItemThrown(CollectableItem item)
        {
            _item = item;
            Destroy(_item.GetComponent<Rigidbody>());
            Destroy(_item.GetComponent<Collider>());
            _item.transform.parent = transform;

            if (_item.TryGetComponent<Renderer>(out var renderer))
            {
               _item.transform.localPosition = new Vector3(0, renderer.bounds.center.y, 0);
            }
            else
                _item.transform.localPosition = Vector3.zero;

            ItemChanged?.Invoke(this);
        }
    }
}