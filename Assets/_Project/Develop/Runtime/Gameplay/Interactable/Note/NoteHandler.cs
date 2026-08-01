using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.NotePopup;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Note
{
    public class NoteHandler : Selectable
    {
        [SerializeField]
        private Sprite _noteImage;
        [SerializeField]
        private string _noteText;

        private NotePopupPresenter _presenter;

        public override string InteractionDescription => Settings.Settings.NOTE_INTERACTION_DESCRIPTION;

        public void Init(NotePopupPresenter presenter)
        {
            _presenter = presenter;
            _presenter.CloseRequest += OnClosed;
        }

        public override void Interact()
        {
            Deselect();
            _presenter.ShowNote(_noteImage, _noteText);
        }

        private void OnDestroy()
        {
            _presenter.CloseRequest -= OnClosed;
        }

        private void OnClosed(PopupPresenterBase _)
        {
            //_collider.enabled = true;
        }
    }
}