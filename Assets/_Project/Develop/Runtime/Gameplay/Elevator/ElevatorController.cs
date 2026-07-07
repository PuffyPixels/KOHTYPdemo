using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Elevator
{
    public class ElevatorController : MonoBehaviour
    {
        [SerializeField] private List<Transform> _elevatorPrefabs;
        private Transform _currentElevatorPrefab;

        private void Awake()
        {
            Assert.IsTrue(_elevatorPrefabs.Count > 0, "_elevatorPrefabs list is empty.");
        }

        public void SetElevator(int elevatorIndex)
        {
            Assert.IsTrue(elevatorIndex >= 0 && elevatorIndex < _elevatorPrefabs.Count,
                $"Elevator index {elevatorIndex} is out of range. Valid range: 0 to {_elevatorPrefabs.Count - 1}");

            if (_currentElevatorPrefab != null && _currentElevatorPrefab.gameObject != null)
            {
                Destroy(_currentElevatorPrefab.gameObject);
                _currentElevatorPrefab = null;
            }

            _currentElevatorPrefab = Instantiate(_elevatorPrefabs[elevatorIndex]);
        }
    }
}
