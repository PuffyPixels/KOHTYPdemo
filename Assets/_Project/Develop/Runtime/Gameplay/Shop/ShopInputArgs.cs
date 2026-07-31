using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;

namespace Assets._Project.Develop.Runtime.Gameplay.Shop
{
    public class ShopInputArgs : IInputSceneArgs
    {
        public ShopInputArgs(DIContainer container)
        {
            GameLogicContainer = container;
        }

        public DIContainer GameLogicContainer { get; }
    }
}
