using Assets._Project.Develop.Runtime.UI.Core;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public enum MainMenuButtons
    {
        ContinueButton,
        StartNewGame,
        Options,
        CloseGame,
    }

    public class MainMenuScreenView : MonoBehaviour, IView
    {
        [field: SerializeField] public MainMenuButtonView ContinueGameButtonView { get; private set; }
        [field: SerializeField] public MainMenuButtonView StartNewGameButtonView { get; private set; }
        [field: SerializeField] public MainMenuButtonView OptionsButtonView { get; private set; }
        [field: SerializeField] public MainMenuButtonView CloseGameButtonView { get; private set; }

        public void SetButtonActive(MainMenuButtons button, bool isActive)
        {
            switch (button)
            {
                case MainMenuButtons.ContinueButton:
                    ContinueGameButtonView.SetActive(isActive);
                    break;
                case MainMenuButtons.StartNewGame:
                    StartNewGameButtonView.SetActive(isActive);
                    break;
                case MainMenuButtons.Options:
                    OptionsButtonView.SetActive(isActive);
                    break;
                case MainMenuButtons.CloseGame:
                    CloseGameButtonView.SetActive(isActive);
                    break;
            }
        }
    }
}
