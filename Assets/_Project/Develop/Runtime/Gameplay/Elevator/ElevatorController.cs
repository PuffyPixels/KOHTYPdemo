using Assets._Project.Develop.Runtime.Gameplay.Interactable.Elevator;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Elevator
{
    public class ElevatorController : MonoBehaviour
    {
        [SerializeField] private List<ElevatorHandler> _elevatorPrefabs;

        private ElevatorHandler _currentElevator;
        private SoundsManager _soundManager;

        private void Awake()
        {
            Assert.IsTrue(_elevatorPrefabs.Count > 0, "_elevatorPrefabs list is empty.");
        }

        public void Init(SoundsManager soundManager)
        {
            Assert.IsNotNull(soundManager);

            _soundManager = soundManager;
        }

        public void SetElevator(int elevatorIndex)
        {
            Assert.IsTrue(elevatorIndex >= 0 && elevatorIndex < _elevatorPrefabs.Count,
                $"Elevator index {elevatorIndex} is out of range. Valid range: 0 to {_elevatorPrefabs.Count - 1}");

            if (_currentElevator != null && _currentElevator.gameObject != null)
            {
                //Destroy(_currentElevator.gameObject);
                _currentElevator = null;
            }

            _currentElevator = Instantiate(_elevatorPrefabs[elevatorIndex]);
            _currentElevator.Init(_soundManager);
        }
    }
}
