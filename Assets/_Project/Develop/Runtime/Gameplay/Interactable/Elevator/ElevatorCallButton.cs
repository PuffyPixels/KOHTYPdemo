using Assets._Project.Develop.Runtime.Utilities.Sound;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Elevator
{
    public class ElevatorCallButton : Selectable
    {
        [SerializeField]
        private AudioClip _buttonClicked;

        private ElevatorHandler _elevatorHandler;
        private SoundsManager _soundsManager;

        public void Init(ElevatorHandler elevatorHandler, SoundsManager soundsManager)
        {
            _elevatorHandler = elevatorHandler;
            _soundsManager = soundsManager;
        }

        public override void Interact()
        {
            _soundsManager.PlaySound(_buttonClicked);
            _elevatorHandler.Call();
            Destroy(this);
        }
    }
}