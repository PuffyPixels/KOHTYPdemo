using Assets._Project.Develop.Runtime.Gameplay.Input;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Project.Develop.Runtime.Utilities.Initializing;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Pause
{
    public class PausePresenter : IPresenter
    {
        private AdditionalInputController _input;
        private PauseView _view;
        private SceneSwitcherService _sceneSwitcher;
        private ICoroutinesPerformer _coroutinesPerformer;
        private bool _isPaused;
        private float _prevTimeScale = 1f;
        private CursorLockMode _prevMode = CursorLockMode.Locked;
        private bool _prevVisible = false;

        public PausePresenter(PauseView view, AdditionalInputController input, SceneSwitcherService sceneSwitcher, ICoroutinesPerformer coroutinePerformer)
        {
            _view = view;
            _input = input;
            _sceneSwitcher = sceneSwitcher;
            _coroutinesPerformer = coroutinePerformer;
        }

        public void Initialize()
        {
            _input.Paused += OnPaused;
            _view.ExitClicked += OnExitClicked;
            _view.ContinueClicked += OnContinueClicked;
        }

        public void Dispose()
        {
            _input.Paused -= OnPaused;
            _view.ExitClicked -= OnExitClicked;
            _view.ContinueClicked -= OnContinueClicked;
        }

        private void OnPaused()
        {
            if (_isPaused)
            {
                OnContinueClicked();
                return;
            }

            _prevTimeScale = Time.timeScale;
            _prevVisible = Cursor.visible;
            _prevMode = Cursor.lockState;

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _isPaused = true;
            _view.Show();
        }

        private void OnContinueClicked()
        {
            Time.timeScale = _prevTimeScale;
            Cursor.lockState = _prevMode;
            Cursor.visible = _prevVisible;
            _isPaused = false;
            _view.Hide();
        }

        private void OnExitClicked()
        {
            Time.timeScale = 1f;
            // BaseSceneEntitiesInitializer.ReloadGame();
            _coroutinesPerformer.StartPerform(_sceneSwitcher.ProcessSwitchTo(Scenes.MainMenu));
        }
    }
}
