using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Assets._Project.Develop.Runtime.Gameplay.Input
{
    public class AdditionalInputController : IDisposable
    {
        public event Action<bool> InventorySwitched;
        public event Action<bool> InventorySelection;
        public event Action InventoryDrop;
        public event Action Paused;

        private PlayerAdditionalInput _controls;
        private bool _isInventoryOpen;

        public AdditionalInputController()
        {
            _controls = new PlayerAdditionalInput();
            _controls.Enable();
            _controls.AdditionalInput.Inventory.performed += OnInventorySwitch;
            _controls.AdditionalInput.Next.performed += OnNext;
            _controls.AdditionalInput.Prev.performed += OnPrev;
            _controls.AdditionalInput.Pause.performed += OnPause;
        }

        public void Dispose()
        {
            _controls.AdditionalInput.Inventory.performed -= OnInventorySwitch;
            _controls.AdditionalInput.Next.performed -= OnNext;
            _controls.AdditionalInput.Prev.performed -= OnPrev;
            _controls.AdditionalInput.Pause.performed -= OnPause;
            _controls.Disable();
        }

        private void OnInventorySwitch(InputAction.CallbackContext _)
        {
            _isInventoryOpen = !_isInventoryOpen;
            InventorySwitched?.Invoke(_isInventoryOpen);
        }

        private void OnNext(InputAction.CallbackContext _)
        {
            InventorySelection?.Invoke(true);
        }

        private void OnPrev(InputAction.CallbackContext context)
        {
            if (context.interaction is HoldInteraction)
                InventoryDrop?.Invoke();
            else if (context.interaction is TapInteraction)
                InventorySelection?.Invoke(false);
        }

        private void OnPause(InputAction.CallbackContext _)
        {
            Paused?.Invoke();
        }
    }
}