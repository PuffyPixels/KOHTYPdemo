using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Door
{
    public class LockedDoorHandler : DoorHandler
    {
        [SerializeField]
        private string _keyName;

        [SerializeField]
        private NavMeshObstacle _obstacle;

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

                if (_obstacle != null)
                    Destroy(_obstacle);
            }

            base.Interact();
        }
    }
}