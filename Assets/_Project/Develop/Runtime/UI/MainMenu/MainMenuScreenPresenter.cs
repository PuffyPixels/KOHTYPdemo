using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenPresenter : IPresenter
    {
        private readonly MainMenuScreenView _screen;

        private SceneSwitcherService _sceneSwitcherService;
        private ICoroutinesPerformer _coroutinesPerformer;

        private readonly List<IPresenter> _childPresenters = new();

        public MainMenuScreenPresenter(
            MainMenuScreenView screen,
            SceneSwitcherService sceneSwitcherService,
            ICoroutinesPerformer coroutinesPerformer)
        {
            _screen = screen;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void Initialize()
        {
            _screen.ContinueGameButtonView.Click += OnContinueGameButtonClicked;
            _screen.StartNewGameButtonView.Click += OnStartNewGameButtonClicked;
            _screen.OptionsButtonView.Click += OnOptionsButtonClicked;
            _screen.CloseGameButtonView.Click += OnCloseGameButtonClicked;

            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();

            InitButtons();
        }

        public void Dispose()
        {
            _screen.ContinueGameButtonView.Click -= OnContinueGameButtonClicked;
            _screen.StartNewGameButtonView.Click -= OnStartNewGameButtonClicked;
            _screen.OptionsButtonView.Click -= OnOptionsButtonClicked;
            _screen.CloseGameButtonView.Click -= OnCloseGameButtonClicked;

            foreach (IPresenter presenter in _childPresenters)
                presenter.Dispose();

            _childPresenters.Clear();
        }

        private void InitButtons()
        {
            _screen.ContinueGameButtonView.SetAvailable(false);
            _screen.StartNewGameButtonView.SetAvailable(true);
            _screen.OptionsButtonView.SetAvailable(false);
            _screen.CloseGameButtonView.SetAvailable(true);
        }

        private void OnContinueGameButtonClicked()
        {
            ButtonClickHandler();
        }

        private void OnStartNewGameButtonClicked()
        {
            ButtonClickHandler();
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.Entrance));
        }

        private void OnOptionsButtonClicked()
        {
            ButtonClickHandler();
        }

        private void OnCloseGameButtonClicked()
        {
            ButtonClickHandler();
            Application.Quit();
        }

        private void ButtonClickHandler()
        {
            _screen.ContinueGameButtonView.SetActive(false);
            _screen.StartNewGameButtonView.SetActive(false);
            _screen.OptionsButtonView.SetActive(false);
            _screen.CloseGameButtonView.SetActive(false);
        }
    }
}
