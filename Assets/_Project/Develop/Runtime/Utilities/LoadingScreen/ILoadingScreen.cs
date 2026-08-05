using System;

namespace Assets._Project.Develop.Runtime.Utilities.LoadingScreen
{
    public interface ILoadingScreen
    {
        bool IsShown { get; }
        void Show();
        void Hide();
        void PlayLoadingSound();
        float GetSoundDuration();

        void FadeIn(float fadeDuration, Action fadedCallback = null);
        void FadeOut(float fadeDuration, Action fadedCallback = null);
    }
}
