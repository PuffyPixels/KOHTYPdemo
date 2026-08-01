using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Elevator
{
    public class ElevatorInputArgs : IInputSceneArgs
    {
        public ElevatorInputArgs(Transform playerSpawnPoint, InventoryItemsDatabase itemsDatabase)
        {
            PlayerSpawnPointPosition = playerSpawnPoint.position;
            PlayerSpawnPointRotation = playerSpawnPoint.rotation;
            ItemsDatabase = itemsDatabase;
        }

        public Vector3 PlayerSpawnPointPosition { get; }
        public Quaternion PlayerSpawnPointRotation { get; }
        public InventoryItemsDatabase ItemsDatabase { get; }
    }
}
