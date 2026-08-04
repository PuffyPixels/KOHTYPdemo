using Assets._Project.Develop.Runtime.Gameplay.Elevator;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Utilities.ElevatorManagment
{
    public class ElevatorSwitchManager
    {
        private ElevatorController _elevator;

        public void AddElevatorController(ElevatorController elevator)
        {
            _elevator = elevator;
        }

        public void ReleaseElevatorController()
        {
            _elevator = null;
        }

        public void SetElevator(int elevatorIndex)
        {
            Assert.IsNotNull(_elevator, "_elevator is null. Make sure ElevatorController is added before calling SetElevator.");

            _elevator.SetElevator(elevatorIndex);
        }
    }
}
