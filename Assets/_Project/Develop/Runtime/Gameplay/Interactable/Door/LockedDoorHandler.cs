using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Door
{
    public class LockedDoorHandler : DoorHandler
    {
        [SerializeField]
        private string _keyName;

        private Inventory _inventory;
        private bool _isLocked = true;

        public override string InteractionDescription => _isLocked ? Settings.Settings.DOOR_LOCKED_INTERACTION_DESCRIPTION : base.InteractionDescription;

        public void Init(Inventory inventory)
        {
            _inventory = inventory;
        }

        public override void Interact()
        {
            if (_isLocked)
            {
                InventoryItem item = _inventory.Items.FirstOrDefault(x => x.Name == _keyName);

                if (item == null)
                {
                    // play locked sound
                    return;
                }

                // play opened sound
                _inventory.Remove(item);
                _isLocked = false;
            }

            base.Interact();
        }
    }
}