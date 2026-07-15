namespace Assets._Project.Develop.Runtime.Gameplay.Interactable
{
    public interface IInteractable
    {
        public void Select() { }

        public void Deselect() { }

        public void Interact();

        public float InteractionDistance => Settings.Settings.BASE_INTERACTION_DISTANCE;
    }
}