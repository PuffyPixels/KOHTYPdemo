using Assets._Project.Develop.Runtime.UI.Core;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour, IView
    {
        [field: SerializeField] public MainMenuButtonView ContinueGameButtonView { get; private set; }
        [field: SerializeField] public MainMenuButtonView StartNewGameButtonView { get; private set; }
        [field: SerializeField] public MainMenuButtonView OptionsButtonView { get; private set; }
        [field: SerializeField] public MainMenuButtonView CloseGameButtonView { get; private set; }
    }
}
