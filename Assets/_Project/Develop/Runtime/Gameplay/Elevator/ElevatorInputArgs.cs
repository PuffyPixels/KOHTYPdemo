using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Elevator
{
    public class ElevatorInputArgs : IInputSceneArgs
    {
        public ElevatorInputArgs(Transform playerSpawnPoint)
        {
            PlayerSpawnPointPosition = playerSpawnPoint.position;
            PlayerSpawnPointRotation = playerSpawnPoint.rotation;
        }

        public Vector3 PlayerSpawnPointPosition { get; }
        public Quaternion PlayerSpawnPointRotation { get; }
    }
}
