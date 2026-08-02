using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.NotePopup;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Note
{
    public class NoteHandler : Selectable
    {
        [SerializeField]
        private string _noteName;

        private NotePopupPresenter _presenter;
        private InventoryItem _noteItem;

        public override string InteractionDescription => Settings.Settings.NOTE_INTERACTION_DESCRIPTION;

        public void Init(InventoryItemsDatabase itemsDatabase, NotePopupPresenter presenter)
        {
            _noteItem = itemsDatabase.GetItem(_noteName);

            if (_noteItem == null)
            {
                Debug.LogWarning($"{gameObject.name} note has no inventory item.");
                Destroy(this);
            }

            _presenter = presenter;
            _presenter.CloseRequest += OnClosed;
        }

        public override void Interact()
        {
            Deselect();
            _presenter.ShowNote(_noteItem);
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