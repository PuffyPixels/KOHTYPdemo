using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Elevator
{
    public class ElevatorCallButton : Selectable
    {
        private ElevatorHandler _elevatorHandler;

        public void Init(ElevatorHandler elevatorHandler)
        {
            _elevatorHandler = elevatorHandler;
        }

        public override void Interact()
        {
            _elevatorHandler.Call();
            Destroy(this);
        }
    }
}