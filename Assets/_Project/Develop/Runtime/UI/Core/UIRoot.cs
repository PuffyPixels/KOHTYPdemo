using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Core
{
    public class UIRoot : MonoBehaviour
    {
        [field: SerializeField] public Transform HUDLayer { get; private set; }
        [field: SerializeField] public Transform VFXUnderPopupsLayer { get; private set; }
        [field: SerializeField] public Transform PopupsLayer { get; private set; }
        [field: SerializeField] public Transform VFXOverPopupsLayer { get; private set; }
    }
}
