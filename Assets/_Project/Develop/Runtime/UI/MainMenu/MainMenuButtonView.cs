using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        public event Action Click;
        public event Action Selected;

        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;

        [SerializeField] private Image _background;
        [SerializeField] private Sprite _selectedBackground;
        [SerializeField] private Sprite _unselectedBackground;

        [SerializeField] private Color _enabledColor = new(0.7529413f, 0.7529413f, 0.7529413f, 1f);
        [SerializeField] private Color _selectedColor = new(0.01f, 0.01f, 0.01f, 1f);
        [SerializeField] private Color _disabledColor = new(0.2f, 0.2f, 0.2f, 1f);

        private bool _isSelected;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void SetActive(bool isActive)
        {
            _button.enabled = isActive;
            enabled = isActive;
        }

        public void SetAvailable(bool isAvailabled)
        {
            _text.faceColor = isAvailabled ? _enabledColor : _disabledColor;
            SetActive(isAvailabled);
        }

        public void OnPointerEnter(PointerEventData eventData) => OnSelected();
        public void OnPointerExit(PointerEventData eventData) => OnDeselected();
        public void OnSelect(BaseEventData eventData) => OnSelected();
        public void OnDeselect(BaseEventData eventData) => OnDeselected();

        private void OnClick() => Click?.Invoke();

        private void OnSelected()
        {
            if (!_isSelected)
            {
                _background.sprite = _selectedBackground;
                _text.faceColor = _selectedColor;
                _isSelected = true;
                Selected?.Invoke();
            }
        }

        private void OnDeselected()
        {
            if (_isSelected)
            {
                _background.sprite = _unselectedBackground;
                _text.faceColor = _enabledColor;
                _isSelected = false;
            }
        }
    }
}
