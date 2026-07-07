using Assets._Project.Develop.Runtime.Gameplay.Elevator;
using System;

namespace Assets._Project.Develop.Runtime.Utilities.ElevatorManagment
{
    public class ElevatorSwitchManager
    {
        private ElevatorController _elevator;

        public void AddElevator(ElevatorController elevator)
        {
            _elevator = elevator;
        }

        public void RemoveElevator()
        {
            _elevator = null;
        }

        public void SetElevator(int elevatorIndex)
        {
            if (_elevator == null)
                throw new ArgumentNullException(nameof(_elevator));

            _elevator.SetElevator(elevatorIndex);
        }
    }
}
