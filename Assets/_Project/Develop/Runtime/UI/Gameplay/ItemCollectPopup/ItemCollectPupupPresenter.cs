using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ItemCollectPopup
{
    public class ItemCollectPupupPresenter : PopupPresenterBase
    {
        public event Action<InventoryItem> ItemCollected;
        public event Action<InventoryItem> ItemDropped;
        public event Action<InventoryItem> ItemFailed;

        private ItemCollectPupupView _view;
        private Inventory _inventory;

        public ItemCollectPupupPresenter(ItemCollectPupupView view, Inventory inventory,
            ICoroutinesPerformer coroutinesPerformer) : base(coroutinesPerformer)
        {
            _view = view;
            _inventory = inventory;
            _inventory.ItemAdded += OnItemAdded;
            _inventory.ItemDropped += OnItemDropped;
            _inventory.ItemFailed += OnItemFailed;
            CloseRequest += HideOnCancel;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Dispose()
        {
            _inventory.ItemAdded -= OnItemAdded;
            _inventory.ItemDropped -= OnItemDropped;
            _inventory.ItemFailed -= OnItemFailed;
            CloseRequest -= HideOnCancel;
            base.Dispose();
        }

        private void HideOnCancel(PopupPresenterBase _) => Hide();

        private void OnItemAdded(Inventory _, InventoryItem item)
        {
            ItemCollected?.Invoke(item);
            Show();
        }

        private void OnItemDropped(Inventory _, InventoryItem item)
        {
            ItemDropped?.Invoke(item);
            Show();
        }

        private void OnItemFailed(Inventory _, InventoryItem item)
        {
            ItemFailed?.Invoke(item);
            Show();
        }
    }
}