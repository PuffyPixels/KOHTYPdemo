using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System.Collections;
using TMPro;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Settings;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using System;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ItemCollectPopup
{
    public class ItemCollectPupupView : PopupViewBase, IDisposable
    {
        [SerializeField]
        Image _icon;
        [SerializeField]
        private TMP_Text _name;

        private ItemCollectPupupPresenter _presenter;
        private CursorManager _cursorManager;
        private ICoroutinesPerformer _coroutinesPerformer;
        private WaitForSeconds _showDelay = new(Settings.ITEM_COLLECT_POPUP_TIME);

        public void Init(ItemCollectPupupPresenter presenter, CursorManager cursorManager, ICoroutinesPerformer coroutinesPerformer)
        {
            _presenter = presenter;
            _cursorManager = cursorManager;
            _coroutinesPerformer = coroutinesPerformer;
            _presenter.ItemCollected += OnItemCollected;
            _presenter.ItemDropped += OnItemDropped;
        }

        public void Close()
        {
            OnCloseButtonClicked();
        }

        protected override void OnPreShow()
        {
            //_cursorManager.ShowCursor(); // На случай если будем закрывать кнопкой
            _coroutinesPerformer.StartPerform(ShowRoutine());
        }

        protected override void OnPostHide()
        {
           // _cursorManager.HideCursor();
            _icon.sprite = null;
            _name.text = "";
        }

        private void OnItemCollected(InventoryItem item)
        {
            _icon.sprite = item.Icon;
            _name.text = item.Name;
        }

        private void OnItemDropped(InventoryItem item)
        {
            _icon.sprite = item.Icon;
            _name.text = $"<color=red>ВЫБРОШЕНО: </color>{item.Name}";
        }

        private IEnumerator ShowRoutine()
        {
            yield return _showDelay;
            Hide();
        }

        public void Dispose()
        {
            _presenter.ItemCollected -= OnItemCollected;
            _presenter.ItemDropped -= OnItemDropped;
        }
    }
}