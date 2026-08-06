using Assets._Project.Develop.Runtime.UI.Core;
using DG.Tweening;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Pause
{
    public class PauseView : MonoBehaviour, IShowableView
    {
        public event Action ContinueClicked;
        public event Action ExitClicked;

        [SerializeField]
        private GameObject _pauseMenu;

        public Tween Show()
        {
            _pauseMenu.SetActive(true);
            return DOTween.Sequence();
        }

        public Tween Hide()
        {
            _pauseMenu.SetActive(false);
            return DOTween.Sequence();
        }

        public void OnContinueClicked() => ContinueClicked?.Invoke();
        public void OnExitClicked() => ExitClicked?.Invoke();

    }
}