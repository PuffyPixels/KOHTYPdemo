using Assets._Project.Develop.Runtime.Gameplay.Enemy.Perekozhnik;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.ElevatorManagment;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Elevator
{
    public class ElevatorHandler : MonoBehaviour
    {
        [SerializeField]
        private Transform _leftDoor, _rightDoor;
        [SerializeField]
        private float _closeOffset;
        [SerializeField]
        private GameObject _exitBlock;
        [SerializeField]
        private bool _isOpened = true;
        [SerializeField]
        private float _targetPositionY;
        [SerializeField]
        private GameObject _doorsImage, _wall;
        [SerializeField]
        private Material _doorsImageMaterial;
        [SerializeField]
        private PerekozhnikFacade _perekozhnik;

        public event Action PlayerEntered, PlayerExited, DoorsOpened, DoorsClosed;

        private float _openLeftX, _openRightX, _closeLeftX, _closeRightX;
        private bool _isPlayerInside;

        private void Awake()
        {
            if (_isOpened)
            {
                _openLeftX = _leftDoor.localPosition.x;
                _openRightX = _rightDoor.localPosition.x;
                _closeLeftX = _openLeftX - _closeOffset;
                _closeRightX = _openRightX + _closeOffset;
            }
            else
            {
                _closeLeftX = _leftDoor.localPosition.x;
                _closeRightX = _rightDoor.localPosition.x;
                _openLeftX = _closeLeftX + _closeOffset;
                _openRightX = _closeRightX - _closeOffset;
            }
        }

        public void Init(SoundsManager soundManager, ICoroutinesPerformer coroutinesPerformer)
        {
            Assert.IsNotNull(soundManager);

            _perekozhnik.Init(soundManager, coroutinesPerformer);
        }

        public IEnumerator OpenDoors()
        {
            if (!_isOpened)
            {
                _leftDoor.DOLocalMoveX(_openLeftX, Settings.Settings.ELEVATOR_DOORS_OPENING_TIME);
                _rightDoor.DOLocalMoveX(_openRightX, Settings.Settings.ELEVATOR_DOORS_OPENING_TIME);
                _isOpened = true;
                _exitBlock.SetActive(false);
                DoorsOpened?.Invoke();
                yield return new WaitForSeconds(Settings.Settings.ELEVATOR_DOORS_OPENING_TIME);
            }
        }

        public IEnumerator CloseDoors()
        {
            if (_isOpened && _isPlayerInside)
            {
                _leftDoor.DOLocalMoveX(_closeLeftX, Settings.Settings.ELEVATOR_DOORS_CLOSING_TIME);
                _rightDoor.DOLocalMoveX(_closeRightX, Settings.Settings.ELEVATOR_DOORS_CLOSING_TIME);
                _isOpened = false;
                _exitBlock.SetActive(true);
                DoorsClosed?.Invoke();
                yield return new WaitForSeconds(Settings.Settings.ELEVATOR_DOORS_CLOSING_TIME);
            }
        }

        public IEnumerator Move()
        {
            if (!_isOpened && _isPlayerInside)
            {
                transform.DOMoveY(_targetPositionY, Settings.Settings.ELEVATOR_MOVING_TIME);
                yield return new WaitForSeconds(Settings.Settings.ELEVATOR_MOVING_TIME);
            }
        }

        public IEnumerator ShowDoorsImageRoutine()
        {
            Renderer renderer = _doorsImage.GetComponent<Renderer>();
            yield return new WaitUntil(() => !_isPlayerInside && !renderer.isVisible);
            _doorsImage.transform.parent = transform.parent;
            renderer.material = _doorsImageMaterial;
            _wall.transform.parent = transform.parent;
            _wall.SetActive(true);
            Destroy(gameObject);
        }

        public void Call()
        {
            StartCoroutine(CallRoutine());
        }

        private IEnumerator CallRoutine()
        {
            yield return new WaitForSeconds(Settings.Settings.ELEVATOR_CALLING_TIME);
            yield return OpenDoors();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                _isPlayerInside = true;
                PlayerEntered?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                _isPlayerInside = false;
                PlayerExited?.Invoke();
            }
        }
    }
}