using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.ElevatorManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System.Collections;

namespace Assets._Project.Develop.Runtime.Gameplay.Shop
{
    public class ShopBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private ElevatorSwitchManager _elevatorSwitchManager;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
        }

        public override IEnumerator Initialize()
        {
            _elevatorSwitchManager = _container.Resolve<ElevatorSwitchManager>();

            yield break;
        }

        public override void Run()
        {
            _elevatorSwitchManager.SetElevator(1);
        }
    }
}
