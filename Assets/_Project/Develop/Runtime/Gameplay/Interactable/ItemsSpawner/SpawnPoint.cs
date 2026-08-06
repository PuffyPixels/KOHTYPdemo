using Assets._Project.Develop.Runtime.Gameplay.Interactable.Item;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.ItemsSpawner
{
    public class SpawnPoint : MonoBehaviour
    {
        [field: SerializeField]
        public ItemType Type { get; private set; }

        public bool IsBusy { get; private set; } = false;

        private int _itemsCount;

        public void Spawn(Inventory inventory, InventoryItemsDatabase itemsDatabase, InventoryItem item, Transform parent = null)
        {
            CollectableItem collectableItem = Instantiate(item.CollectableItem, transform.position, transform.rotation, parent: parent);

            if (collectableItem != null)
                collectableItem.Init(inventory, itemsDatabase);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Selectable"))
                _itemsCount++;

            IsBusy = _itemsCount > 0;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Selectable"))
            {
                _itemsCount--;

                if (_itemsCount < 0)
                    _itemsCount = 0;
            }

            IsBusy = _itemsCount > 0;
        }
    }
}