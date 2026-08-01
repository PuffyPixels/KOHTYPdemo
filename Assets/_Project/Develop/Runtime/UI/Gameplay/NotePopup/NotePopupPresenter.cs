using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.NotePopup
{
    public class NotePopupPresenter : PopupPresenterBase
    {
        private NotePopupView _view;
        private CursorManager _cursorManager;

        public NotePopupPresenter(NotePopupView view, CursorManager cursorManager, ICoroutinesPerformer coroutinesPerformer) : base(coroutinesPerformer)
        {
            _view = view;
            _cursorManager = cursorManager;
        }

        protected override PopupViewBase PopupView => _view;

        public void ShowNote(Sprite image, string text)
        {
            _view.SetNote(image, text);
            CloseRequest += OnClosed;
            Show();
        }

        public override void Dispose()
        {
            base.Dispose();
            CloseRequest -= OnClosed;
        }

        protected override void OnPreShow()
        {
            base.OnPreShow();
            Time.timeScale = 0f;
            _cursorManager.ShowCursor();
        }

        protected override void OnPostHide()
        {
            base.OnPostHide();
            Time.timeScale = 1f;
            _cursorManager.HideCursor();
        }

        private void OnClosed(PopupPresenterBase _)
        {
            Hide();
        }
    }
}