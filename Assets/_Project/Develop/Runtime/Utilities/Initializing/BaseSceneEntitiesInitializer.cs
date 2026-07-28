using Assets._Project.Develop.Runtime.Gameplay.Interactable.Item;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
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
    }
}