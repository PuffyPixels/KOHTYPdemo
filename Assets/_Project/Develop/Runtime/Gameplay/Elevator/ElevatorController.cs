using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Elevator
{
    public class ElevatorController : MonoBehaviour
    {
        [SerializeField] private List<Transform> _elevatorPrefabs;
        private Transform _currentElevatorPrefab;

        private void Awake()
        {
            if (_elevatorPrefabs.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(_elevatorPrefabs));
        }

        public void SetElevator(int elevatorIndex)
        {
            if (elevatorIndex < 0 || elevatorIndex >= _elevatorPrefabs.Count)
                throw new ArgumentOutOfRangeException(nameof(elevatorIndex), $"Index must be between 0 and {_elevatorPrefabs.Count - 1}");

            if (_currentElevatorPrefab != null && _currentElevatorPrefab.gameObject != null)
            {
                Destroy(_currentElevatorPrefab.gameObject);
                _currentElevatorPrefab = null;
            }

            _currentElevatorPrefab = Instantiate(_elevatorPrefabs[elevatorIndex]);
        }
    }
}
