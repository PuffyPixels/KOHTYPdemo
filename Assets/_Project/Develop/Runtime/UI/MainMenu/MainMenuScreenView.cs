using Assets._Project.Develop.Runtime.UI.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour, IView
    {
        public event Action StartNewGameButtonClicked;
        public event Action CloseGameButtonClicked;

        [SerializeField] private Button _startNewGameButton;
        [SerializeField] private Button _closeGameButton;

        private void OnEnable()
        {
            _startNewGameButton.onClick.AddListener(OnStartNewGameButtonClicked);
            _closeGameButton.onClick.AddListener(OnCloseGameButtonClicked);
        }

        private void OnDisable()
        {
            _startNewGameButton.onClick.RemoveListener(OnStartNewGameButtonClicked);
            _closeGameButton.onClick.RemoveListener(OnCloseGameButtonClicked);
        }

        private void OnStartNewGameButtonClicked() => StartNewGameButtonClicked?.Invoke();
        private void OnCloseGameButtonClicked() => CloseGameButtonClicked?.Invoke();
    }
}
