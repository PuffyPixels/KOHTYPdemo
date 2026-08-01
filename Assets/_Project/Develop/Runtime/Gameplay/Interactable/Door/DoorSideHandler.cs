using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Door
{
    public class DoorSideHandler : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private DoorHandler _doorHandler;
        [SerializeField]
        private Collider _collider;

        public void Select() => _doorHandler.Select();

        public void Deselect() => _doorHandler.Deselect();

        public string InteractionDescription => _doorHandler.InteractionDescription;

        public void Interact()
        {
            _doorHandler.Push(this);
            _doorHandler.Interact();
        }

        public void SetDoorReady(bool isDoorReady)
        {
            _collider.enabled = isDoorReady;
        }
    }
}
