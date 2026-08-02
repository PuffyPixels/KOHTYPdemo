using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Door
{
    public class DoorAutoPusher : MonoBehaviour
    {
        [SerializeField]
        private DoorHandler _doorHandler;
        [SerializeField]
        private DoorSideHandler _doorSideHandler;

        private void OnTriggerEnter(Collider other)
        {
            if (!_doorHandler.IsOpen)
            {
                _doorSideHandler.Interact();
            }
        }
    }
}