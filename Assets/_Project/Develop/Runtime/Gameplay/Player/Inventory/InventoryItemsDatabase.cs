using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Inventory
{
    [CreateAssetMenu(fileName = "InventoryItemsDatabase", menuName = "Scriptable Objects/InventoryItemsDatabase")]
    public class InventoryItemsDatabase : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItem> _items;

        public InventoryItem GetItem(string name) => _items.FirstOrDefault(x => x.Name.Equals(name));
    }
}