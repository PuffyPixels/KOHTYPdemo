using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenPresenter : IPresenter
    {
        private const int BACKGROUND_SWITCH = 2;

        private readonly MainMenuScreenView _screen;

        private SceneSwitcherService _sceneSwitcherService;
        private ICoroutinesPerformer _coroutinesPerformer;
        private SoundsManager _soundManager;

        private readonly List<IPresenter> _childPresenters = new();

        private Sprite _normal;
        private Sprite _faded;

        private WaitForSeconds _backgroundWait = new(0.08f);

        public MainMenuScreenPresenter(
            MainMenuScreenView screen,
            SceneSwitcherService sceneSwitcherService,
            ICoroutinesPerformer coroutinesPerformer,
            SoundsManager soundManager)
        {
            _screen = screen;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _soundManager = soundManager;
        }

        public void Initialize()
        {
            _screen.ContinueGameButtonView.Click += OnContinueGameButtonClicked;
            _screen.StartNewGameButtonView.Click += OnStartNewGameButtonClicked;
            _screen.OptionsButtonView.Click += OnOptionsButtonClicked;
            _screen.CloseGameButtonView.Click += OnCloseGameButtonClicked;

            _screen.ContinueGameButtonView.Selected += OnSelected;
            _screen.StartNewGameButtonView.Selected += OnSelected;
            _screen.OptionsButtonView.Selected += OnSelected;
            _screen.CloseGameButtonView.Selected += OnSelected;

            _normal = _screen.Normal;
            _faded = _screen.Faded;
            _screen.Background.sprite = _normal;

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

            _screen.ContinueGameButtonView.Selected -= OnSelected;
            _screen.StartNewGameButtonView.Selected -= OnSelected;
            _screen.OptionsButtonView.Selected -= OnSelected;
            _screen.CloseGameButtonView.Selected -= OnSelected;

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
            _coroutinesPerformer.StartPerform(BackgroundSwitch());
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

        private void OnSelected()
        {
            _soundManager.PlaySound(_screen.Selected, is2D: true);
        }

        private void ButtonClickHandler()
        {
            _soundManager.PlaySound(_screen.Clicked, is2D: true);

            _screen.ContinueGameButtonView.SetActive(false);
            _screen.StartNewGameButtonView.SetActive(false);
            _screen.OptionsButtonView.SetActive(false);
            _screen.CloseGameButtonView.SetActive(false);
        }

        private IEnumerator BackgroundSwitch()
        {
            for (int i = 0; i < BACKGROUND_SWITCH; i++)
            {
                _screen.Background.sprite = _faded;
                yield return _backgroundWait;

                _screen.Background.sprite = _normal;
                yield return _backgroundWait;
            }
        }
    }
}
