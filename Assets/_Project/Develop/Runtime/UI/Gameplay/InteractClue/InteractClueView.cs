using Assets._Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.UI.Gameplay.InteractClue
{
    public class InteractClueView : PopupViewBase
    {
        [SerializeField]
        private TMP_Text _description;

        public string Description
        {
            private get => _description.text;
            set => _description.text = $"[E] {value}";
        }

        protected override void OnPreShow()
        {
            _description.enabled = true;
        }

        protected override void OnPreHide()
        {
            _description.enabled = false;
        }
    }
}