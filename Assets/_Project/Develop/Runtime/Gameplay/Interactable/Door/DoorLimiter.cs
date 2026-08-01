using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Door
{
    public class DoorLimiter : MonoBehaviour
    {
        [field: SerializeField]
        public bool IsFront { get; private set; }
    }
}