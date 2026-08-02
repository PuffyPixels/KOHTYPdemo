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

        public void Spawn(InventoryItem item, Transform parent = null)
        {
            Instantiate(item.CollectableItem, transform.position, transform.rotation, parent: parent);
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