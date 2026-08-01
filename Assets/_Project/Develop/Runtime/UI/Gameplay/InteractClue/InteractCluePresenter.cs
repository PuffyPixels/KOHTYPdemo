using Assets._Project.Develop.Runtime.Gameplay.Interactable;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.UI.Gameplay.InteractClue
{
    public class InteractCluePresenter : PopupPresenterBase, IDisposable
    {
        private readonly InteractClueView _view;
        private InteractionHandler _interactionHandler;

        public InteractCluePresenter(InteractClueView view, InteractionHandler interactionHandler, 
            ICoroutinesPerformer coroutinesPerformer) : base(coroutinesPerformer)
        {
            _view = view;
            _interactionHandler = interactionHandler;
            _interactionHandler.Selected += OnSelected;
            _interactionHandler.Deselected += OnDeselected;
        }

        public override void Dispose()
        {
            base.Dispose();
            _interactionHandler.Selected -= OnSelected;
            _interactionHandler.Deselected -= OnDeselected;
        }

        protected override PopupViewBase PopupView => _view;

        private void OnSelected(IInteractable interactable)
        {
            if (!string.IsNullOrEmpty(interactable.InteractionDescription))
            {
                _view.Description = interactable.InteractionDescription;
                Show();
            }
        }

        private void OnDeselected()
        {
            Hide();
        }
    }
}
