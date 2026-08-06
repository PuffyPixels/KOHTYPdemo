using Assets._Project.Develop.Runtime.Gameplay.Elevator;
using Assets._Project.Develop.Runtime.Gameplay.Input;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.InteractClue;
using Assets._Project.Develop.Runtime.UI.Gameplay.InventoryWidget;
using Assets._Project.Develop.Runtime.UI.Gameplay.ItemCollectPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.NotePopup;
using Assets._Project.Develop.Runtime.UI.Pause;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class ElevatorContextRegistrations
    {
        private static ElevatorInputArgs _inputArgs;

        public static void Process(DIContainer container, ElevatorInputArgs inputArgs)
        {
            _inputArgs = inputArgs;

            container.RegisterAsSingle(CreateGameplayUIRoot);//.NonLazy();
            container.RegisterAsSingle(CreateGameplayScreenPresenter);//.NonLazy();
            container.RegisterAsSingle(CreateInventory);
            container.RegisterAsSingle(CreateInventoryItemsDatabase);
            container.RegisterAsSingle(CreateItemCollectPopupPresenter);//.NonLazy();
            container.RegisterAsSingle(CreateInventoryWidgetPresenter);//.NonLazy();
            container.RegisterAsSingle(CreateInteractCluePresenter);
            container.RegisterAsSingle(CreateNotePopupPresenter);
            container.RegisterAsSingle(CreatePausePresenter);
            container.RegisterAsSingle(_ => new AdditionalInputController()).NonLazy();
            container.RegisterAsSingle(HeroFactory);
            container.RegisterAsSingle(CreateGameplayPresentersFactory);
        }

        private static HeroFactory HeroFactory(DIContainer c)
            => new(c, _inputArgs);

        private static Inventory CreateInventory(DIContainer _) 
            => new(Settings.Settings.INVENTORY_CAPACITY);

        private static InventoryItemsDatabase CreateInventoryItemsDatabase(DIContainer _) =>
            _inputArgs.ItemsDatabase;

        private static UIRoot CreateGameplayUIRoot(DIContainer c)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();

            UIRoot gameplayUIRootPrefab = resourcesAssetsLoader
                .Load<UIRoot>("UI/UIRoot");

            return UnityEngine.Object.Instantiate(gameplayUIRootPrefab);
        }

        private static GameplayScreenPresenter CreateGameplayScreenPresenter(DIContainer c) =>
            CreateHUDWidget<GameplayScreenPresenter, GameplayScreenView>(c, ViewIDs.GameplayScreen,
                v => c.Resolve<GameplayPresentersFactory>().CreateGameplayScreenPresenter(v));

        private static ItemCollectPupupPresenter CreateItemCollectPopupPresenter(DIContainer c) =>
            CreateHUDWidget<ItemCollectPupupPresenter, ItemCollectPupupView>(c, ViewIDs.ItemCollectPopupWidget,
                v =>
                {
                    var presenter = c.Resolve<GameplayPresentersFactory>().CreateItemCollectPopupPresenter(v);
                    v.Init(presenter, c.Resolve<CursorManager>(), c.Resolve<ICoroutinesPerformer>());
                    return presenter;
                });

        private static InventoryWidgetPresenter CreateInventoryWidgetPresenter(DIContainer c) =>
            CreateHUDWidget<InventoryWidgetPresenter, InventoryWidgetView>(c, ViewIDs.InventoryWidget,
                v => c.Resolve<GameplayPresentersFactory>().CreateInventoryWidgetPresenter(v));

        private static InteractCluePresenter CreateInteractCluePresenter(DIContainer c) =>
            CreateHUDWidget<InteractCluePresenter, InteractClueView>(c, ViewIDs.InteractClueWidget,
                v => c.Resolve<GameplayPresentersFactory>().CreateInteractCluePresenter(v));

        private static NotePopupPresenter CreateNotePopupPresenter(DIContainer c) =>
            CreateHUDWidget<NotePopupPresenter, NotePopupView>(c, ViewIDs.NotePopupWidget,
                v => c.Resolve<GameplayPresentersFactory>().CreateNotePopupPresenter(v));

        private static PausePresenter CreatePausePresenter(DIContainer c) =>
            CreateHUDWidget<PausePresenter, PauseView>(c, ViewIDs.PauseWidget,
                v => c.Resolve<GameplayPresentersFactory>().CreatePausePresenter(v));

        private static TPresenter CreateHUDWidget<TPresenter, TView>(DIContainer c, string viewID, 
            Func<TView, TPresenter> presenterCreator) 
            where TView : MonoBehaviour, IView
            where TPresenter : IPresenter
        {
            UIRoot uiRoot = c.Resolve<UIRoot>();

            TView view = c
                .Resolve<ViewsFactory>()
                .Create<TView>(viewID, uiRoot.HUDLayer);

            return presenterCreator(view);
        }

        private static GameplayPresentersFactory CreateGameplayPresentersFactory(DIContainer c)
        {
            return new GameplayPresentersFactory(c);
        }
    }
}
