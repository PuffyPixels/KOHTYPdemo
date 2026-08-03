using Assets._Project.Develop.Runtime.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour, IView
    {
        [field: SerializeField] public MainMenuButtonView ContinueGameButtonView { get; private set; }
        [field: SerializeField] public MainMenuButtonView StartNewGameButtonView { get; private set; }
        [field: SerializeField] public MainMenuButtonView OptionsButtonView { get; private set; }
        [field: SerializeField] public MainMenuButtonView CloseGameButtonView { get; private set; }
        [field: SerializeField] public Image Background { get; private set; }
        [field: SerializeField] public Sprite Normal { get; private set; }
        [field: SerializeField] public Sprite Faded { get; private set; }
        [field: SerializeField] public AudioClip Clicked { get; private set; }
        [field: SerializeField] public AudioClip Selected { get; private set; }
    }
}
