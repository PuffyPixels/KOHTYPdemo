using Assets._Project.Develop.Runtime.Gameplay.Interactable.FakeTv;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using DG.Tweening;
using DyrdaDev.FirstPersonController;
using Project.Develop.Runtime.Utilities.Initializing;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Elevator
{
    public class FinalElevator : MonoBehaviour
    {
        [SerializeField]
        private ElevatorHandler _elevatorHandler;
        [SerializeField]
        private Transform _lookOutPoint;
        [SerializeField]
        private Transform _insidePoint;

        FakeTvHandler _fakeTv;

        public void Init(FakeTvHandler fakeTvHandler)
        {
            _fakeTv = fakeTvHandler;
            _fakeTv.Taken += OnTvTaken;
            _elevatorHandler.PlayerEntered += OnPlayerEntered;
        }


        private IEnumerator FinalRoutine(Transform player)
        {
            yield return player.DOLookAt(_insidePoint.position, Settings.Settings.ELEVATOR_LOOK_TIME).WaitForCompletion();
            yield return player.DOMove(_insidePoint.position, Settings.Settings.ELEVATOR_MOVING_TIME).WaitForCompletion();
            yield return player.DOLookAt(_lookOutPoint.position, Settings.Settings.ELEVATOR_LOOK_TIME).WaitForCompletion();
            yield return _elevatorHandler.CloseDoors();
            BaseSceneEntitiesInitializer.ReloadGame();
        }

        private void OnTvTaken()
        {
            _fakeTv.Taken -= OnTvTaken;
            StartCoroutine(_elevatorHandler.OpenDoors());
        }

        private void OnPlayerEntered()
        {
            _elevatorHandler.PlayerEntered -= OnPlayerEntered;
            var input = GameObject.FindFirstObjectByType<InputActionBasedFirstPersonControllerInput>();

            if (input != null)
            {
                input.enabled = false;
                StartCoroutine(FinalRoutine(input.transform));
            }
        }

        private void OnDestroy()
        {
            _fakeTv.Taken -= OnTvTaken;
            _elevatorHandler.PlayerEntered -= OnPlayerEntered;
        }
    }
}
