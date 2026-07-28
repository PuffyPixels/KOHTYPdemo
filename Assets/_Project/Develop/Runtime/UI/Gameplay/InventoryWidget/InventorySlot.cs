using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.InventoryWidget
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        public InventoryItem Item { get; private set; }

        public void SetItem(InventoryItem item)
        {
            Item = item ?? throw new System.ArgumentNullException(nameof(item));
            _image.sprite = Item.Icon;
            _image.enabled = true;
        }

        public void Clear()
        {
            _image.sprite = null;
            _image.enabled = false;
        }

        public void Select()
        {
            transform.localScale = new(1.1f, 1.1f, 1f);
        }

        public void Deselect()
        {
            transform.localScale = new(1f, 1f, 1f);
        }
    }
}
