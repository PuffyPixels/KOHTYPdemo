using Assets._Project.Develop.Runtime.Gameplay.Interactable;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.Door;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.Elevator;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.Item;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.Note;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Gameplay.NotePopup;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System.Linq;
using UnityEngine;

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
            ICoroutinesPerformer coroutinesPerformer, DIContainer gameContainer, Transform hero)
        {
            ElevatorPanel panel = GameObject.FindFirstObjectByType<ElevatorPanel>();

            if (panel != null)
            {
                panel.Init(sceneLoaderService, sceneSwitcherService, coroutinesPerformer, gameContainer, hero);
            }
        }

        public static void InitNotes(NotePopupPresenter presenter)
        {
            GameObject.FindObjectsByType<NoteHandler>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().
                ForEach(x => x.Init(presenter));
        }

        public static void InitLockedDoors(Inventory inventory)
        {
            GameObject.FindObjectsByType<LockedDoorHandler>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().
                ForEach(x => x.Init(inventory));
        }
    }
}