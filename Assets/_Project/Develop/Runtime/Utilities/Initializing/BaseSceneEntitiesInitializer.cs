using Assets._Project.Develop.Runtime.Gameplay.Interactable.Door;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.Elevator;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.Item;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.ItemStorage;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.Note;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Gameplay.NotePopup;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.ElevatorManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Assets._Project.Develop.Runtime.Utilities.Remover;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.FakeTv;

namespace Project.Develop.Runtime.Utilities.Initializing
{
    public static class BaseSceneEntitiesInitializer
    {
        public static void InitCollectableObjects(Inventory inventory, InventoryItemsDatabase inventoryItemsDatabase)
        {
            GameObject.FindObjectsByType<CollectableItem>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().
                ForEach(x => x.Init(inventory, inventoryItemsDatabase)) ;
        }

        public static void InitElevatorPanel(SceneLoaderService sceneLoaderService, SceneSwitcherService sceneSwitcherService,
            ICoroutinesPerformer coroutinesPerformer, DIContainer gameContainer, Hero hero, ElevatorSwitchManager elevatorSwitchManager)
        {
            ElevatorPanel panel = GameObject.FindFirstObjectByType<ElevatorPanel>();

            if (panel != null)
            {
                panel.Init(sceneLoaderService, sceneSwitcherService, coroutinesPerformer, gameContainer, hero, elevatorSwitchManager);
            }
        }

        public static void InitElevatorCallButton()
        {
            ElevatorHandler handler = GameObject.FindFirstObjectByType<ElevatorHandler>();
            ElevatorCallButton button = GameObject.FindFirstObjectByType<ElevatorCallButton>();

            if (handler != null && button != null)
            {
                button.Init(handler);
            }
        }

        public static void InitNotes(InventoryItemsDatabase itemsDatabase, NotePopupPresenter presenter)
        {
            GameObject.FindObjectsByType<NoteHandler>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().
                ForEach(x => x.Init(itemsDatabase, presenter));
        }

        public static void InitLockedDoors(Inventory inventory)
        {
            GameObject.FindObjectsByType<LockedDoorHandler>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().
                ForEach(x => x.Init(inventory));
        }

        public static void InitItemsStorages(ItemThrower itemThrower)
        {
            GameObject.FindObjectsByType<ItemStorage>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().
                ForEach(x => x.Init(itemThrower));
        }

        public static void InitFinalElevator()
        {
            FakeTvHandler fakeTv = GameObject.FindFirstObjectByType<FakeTvHandler>(FindObjectsInactive.Include);
            FinalElevator finalElevator = GameObject.FindFirstObjectByType<FinalElevator>(FindObjectsInactive.Include);

            if (finalElevator != null)
                finalElevator.Init(fakeTv);
        }

        public static void ReloadGame()
        {
            Remover.ClearDontDestroyAndLoad();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(Scenes.GameEntryPoint);
        }
    }
}