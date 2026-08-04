using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Shop
{
    [Serializable]
    public class ConsultantSettings
    {
        [field: SerializeField] public ConsultantFacade Prefab {  get; private set; }
        [field: SerializeField] public List<Transform> SpawnPoints { get; private set; }
    }
}
