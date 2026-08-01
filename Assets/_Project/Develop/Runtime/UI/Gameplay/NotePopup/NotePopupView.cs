using Assets._Project.Develop.Runtime.UI.Core;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.NotePopup
{
    public class NotePopupView : PopupViewBase
    {
        [SerializeField]
        private Image _noteImage;
        [SerializeField]
        private TMP_Text _noteText;

        public void SetNote(Sprite image, string text)
        {
            _noteImage.sprite = image;
            _noteText.text = text;
        }

        public void Close()
        {
            OnCloseButtonClicked();
        }

        protected override void OnPostHide()
        {
            _noteImage.sprite = null;
            _noteText.text = "";
        }
    }
}
