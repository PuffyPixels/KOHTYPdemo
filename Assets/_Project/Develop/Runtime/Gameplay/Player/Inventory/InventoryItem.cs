using Assets._Project.Develop.Runtime.Gameplay.Interactable.Item;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Inventory
{
    public enum ItemType
    {
        Item,
        Key,
        Note
    }

    [Serializable]
    public class InventoryItem
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public ItemType Type { get; private set; } = ItemType.Item;
        [field: SerializeField]
        public string Description { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [field: SerializeField]
        public CollectableItem CollectableItem { get; private set; }

        public InventoryItem(string name, string description, Sprite icon)
        {
            Name = name;
            Description = description;
            Icon = icon;
        }
    }
}