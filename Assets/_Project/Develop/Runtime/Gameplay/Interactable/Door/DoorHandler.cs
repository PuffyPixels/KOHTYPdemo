using Assets._Project.Develop.Runtime.Utilities.Sound;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Door
{
    public class DoorHandler : Selectable
    {
        [SerializeField]
        private DoorSideHandler _frontSideHandler, _backSideHandler;
        [SerializeField]
        private Vector3 _rotateAxis = new(0, 0, 1);
        [SerializeField]
        private float _openDeltaAngle = 90f;
        [SerializeField]
        private EnvironmentSound _openSound;

        public bool IsOpen { get; private set; }

        private DoorSideHandler _currentPush;
        private Vector3 _closedAngles;
        private Vector3 _openFrontAngles, _openBackAngles;
        private bool _isMoving;

        private void Awake()
        {
            _closedAngles = transform.localEulerAngles;
            _openFrontAngles = (transform.localRotation * Quaternion.AngleAxis(-_openDeltaAngle, _rotateAxis)).eulerAngles;
            _openBackAngles = (transform.localRotation * Quaternion.AngleAxis(_openDeltaAngle, _rotateAxis)).eulerAngles;
        }

        public override string InteractionDescription
        {
            get
            {
                if (_isMoving)
                    return string.Empty;

                return IsOpen ? Settings.Settings.DOOR_CLOSE_INTERACTION_DESCRIPTION : 
                    Settings.Settings.DOOR_OPEN_INTERACTION_DESCRIPTION;
            }
        }

        public void Push(DoorSideHandler sideHandler)
        {
            _currentPush = sideHandler;
        }

        public override void Interact()
        {
            if (_isMoving)
                return;

            if (IsOpen)
                Move(_closedAngles, false);
            else
                Move(_currentPush == _frontSideHandler ? _openFrontAngles : _openBackAngles, true);
        }

        private void Move(Vector3 target, bool isOpening)
        {
            _openSound.PlaySound();
            _isMoving = true;
            _frontSideHandler.SetDoorReady(false);
            _backSideHandler.SetDoorReady(false);

            transform.DOLocalRotate(target, Settings.Settings.DOOR_MOVING_TIME).OnComplete(() =>
                {
                    IsOpen = isOpening;
                    _isMoving = false;
                    _frontSideHandler.SetDoorReady(true);
                    _backSideHandler.SetDoorReady(true);
                });
        }
    }
}
