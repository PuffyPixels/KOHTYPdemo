using Assets._Project.Develop.Runtime.Gameplay.Enemy;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.ItemsSpawner;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Shop
{
    public class ShopContextRegistrations
    {
        private static ShopInputArgs _inputArgs;

        public static void Process(DIContainer container, ShopInputArgs inputArgs)
        {
            _inputArgs = inputArgs;

            container.RegisterAsSingle(CreateEmeniesFactory);

            container.RegisterAsSingle(CreateBrainsFactory);

            container.RegisterAsSingle(CreateGameProgress);

            container.RegisterAsSingle(CreateItemsSpawner);
        }

        private static EnemiesFactory CreateEmeniesFactory(DIContainer c)
        {
            Camera heroCamera = _inputArgs.Hero.GetComponentInChildren<Camera>();

            return new(c, heroCamera);
        }
            

        private static BrainsFactory CreateBrainsFactory(DIContainer c)
        {
            Inventory inventory = _inputArgs.GameLogicContainer.Resolve<Inventory>();

            return new(c, inventory);
        }

        private static GameProgress CreateGameProgress(DIContainer c)
        {
            Inventory inventory = _inputArgs.GameLogicContainer.Resolve<Inventory>();

            return new(inventory);
        }

        private static ItemsSpawner CreateItemsSpawner(DIContainer _) => GameObject.FindFirstObjectByType<ItemsSpawner>();
    }
}
