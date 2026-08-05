using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.Gameplay.Shop
{
    public class ShopInputArgs : IInputSceneArgs
    {
        public ShopInputArgs(DIContainer container, Hero hero)
        {
            GameLogicContainer = container;
            Hero = hero;
        }

        public DIContainer GameLogicContainer { get; }
        public Hero Hero { get; }
    }
}
