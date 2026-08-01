using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable
{
    public abstract class Selectable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Outline _outline;

        public virtual string InteractionDescription => Settings.Settings.SELECTABLE_INTERACTION_DESCRIPTION;

        public virtual void Select()
        {
            if (_outline != null)
                _outline.enabled = true;
        }

        public virtual void Deselect()
        {
            if (_outline != null)
                _outline.enabled = false;
        }

        public virtual float InteractionDistance => Settings.Settings.BASE_INTERACTION_DISTANCE;

        public abstract void Interact();
    }
}