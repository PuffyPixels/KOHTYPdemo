using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Item
{
    public class CollectableItem : Selectable
    {
        [SerializeField]
        private string _name;

        private Inventory _inventory;
        private InventoryItemsDatabase _itemsDatabase;

        public void Init(Inventory inventory, InventoryItemsDatabase itemsDatabase)
        {
            _inventory = inventory;
            _itemsDatabase = itemsDatabase;
        }

        public override string InteractionDescription => Settings.Settings.COLLECTABLE_INTERACTION_DESCRIPTION;

        public override void Interact()
        {
            if (_inventory.TryAdd(_itemsDatabase.GetItem(_name)))
            {
                OnInteract();
                Destroy(gameObject);
            }
        }

        protected virtual void OnInteract() { }
    }
}