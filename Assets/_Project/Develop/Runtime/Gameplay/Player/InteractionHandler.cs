using Assets._Project.Develop.Runtime.Gameplay.Interactable;
using DyrdaDev.FirstPersonController;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Player
{
    public class InteractionHandler : MonoBehaviour
    {
        [SerializeField]
        private FirstPersonController _controller;
        [SerializeField]
        private LayerMask _interactionLayer;

        private IInteractable _currentInteractable;
        private readonly Vector3 _rayStart = new(0.5f, 0.5f, 0f);
        private readonly WaitForSeconds _checkForInteractionAbilityDelay = new(Settings.Settings.CHECK_INTERACTION_INTERVAL);


        private void Start()
        {
            _controller.Used.Subscribe(_ => { OnUsed(); }).AddTo(this);
        }

        private void OnUsed()
        {
            if (FindInteractable(out IInteractable interactable))
                interactable.Interact();
        }

        private IEnumerator CheckForInteractionAbilityRoutine()
        {
            while (true)
            {
                if (FindInteractable(out IInteractable interactable))
                {
                    if (_currentInteractable != interactable)
                    {
                        ResetInteractable();
                        _currentInteractable = interactable;
                        _currentInteractable.Select();
                    }
                }
                else
                    ResetInteractable();

                yield return _checkForInteractionAbilityDelay;
            }
        }

        private void ResetInteractable()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Deselect();
                _currentInteractable = null;
            }
        }

        private bool FindInteractable(out IInteractable interactable)
        {
            Ray ray = Camera.main.ViewportPointToRay(_rayStart);
            interactable = null;

            return Physics.Raycast(ray, out var hit, Settings.Settings.MAX_INTERACTION_DISTANCE, _interactionLayer) &&
                hit.collider.TryGetComponent(out interactable) && hit.distance <= interactable.InteractionDistance;
        }

        private void OnEnable()
        {
            StartCoroutine(CheckForInteractionAbilityRoutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}