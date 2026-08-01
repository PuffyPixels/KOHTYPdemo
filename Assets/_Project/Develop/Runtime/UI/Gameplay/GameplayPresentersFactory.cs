using Assets._Project.Develop.Runtime.Gameplay.Input;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.InteractClue;
using Assets._Project.Develop.Runtime.UI.Gameplay.InventoryWidget;
using Assets._Project.Develop.Runtime.UI.Gameplay.ItemCollectPopup;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPresentersFactory
    {
        private readonly DIContainer _container;

        public GameplayPresentersFactory(DIContainer container)
        {
            _container = container;
        }

        public GameplayScreenPresenter CreateGameplayScreenPresenter(GameplayScreenView view)
        {
            return new GameplayScreenPresenter(
                view, 
                _container.Resolve<GameplayPresentersFactory>(),
                _container.Resolve<ProjectPresentersFactory>());
        }

        public ItemCollectPupupPresenter CreateItemCollectPopupPresenter(ItemCollectPupupView view)
        {
            return new ItemCollectPupupPresenter(view,
                _container.Resolve<Inventory>(),
                _container.Resolve<ICoroutinesPerformer>());
        }

        public InventoryWidgetPresenter CreateInventoryWidgetPresenter(InventoryWidgetView view)
        {
            return new InventoryWidgetPresenter(_container.Resolve<Inventory>(), view, _container.Resolve<AdditionalInputController>());
        }

        public InteractCluePresenter CreateInteractCluePresenter(InteractClueView view)
        {
            InteractionHandler interactionHandler = GameObject.FindFirstObjectByType<InteractionHandler>();
            return new InteractCluePresenter(view, interactionHandler, _container.Resolve<ICoroutinesPerformer>());
        }
    }
}
