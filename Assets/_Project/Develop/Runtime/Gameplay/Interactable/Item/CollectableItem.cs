using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Item
{
    public class CollectableItem : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Outline _outline;
        private Inventory _inventory;
        private InventoryItem _inventoryItem;

        public void Construct(Inventory inventory, InventoryItem inventoryItem)
        {
            _inventory = inventory;
            _inventoryItem = inventoryItem;
        }   

        public void Select()
        {
            if (_outline != null)
                _outline.enabled = true;
        }

        public void Deselect()
        {
            if (_outline != null)
                _outline.enabled = false;
        }

        public void Interact()
        {
            _inventory.Add(_inventoryItem);
            OnInteract();
            Destroy(gameObject);
        }

        protected virtual void OnInteract() { }
    }
}