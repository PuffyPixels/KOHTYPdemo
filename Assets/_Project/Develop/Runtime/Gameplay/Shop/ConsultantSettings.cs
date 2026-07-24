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
        [field: SerializeField] public Transform Parent { get; private set; }

        [Header("Sounds")]
        [field: SerializeField] public List<AudioClip> AnabiosisSounds { get; private set; }
        [field: SerializeField] public List<AudioClip> PatrolSounds { get; private set; }
        [field: SerializeField] public List<AudioClip> AttentionSounds { get; private set; }
        [field: SerializeField] public List<AudioClip> InvestigateSounds { get; private set; }
        [field: SerializeField] public List<AudioClip> ChaseSounds { get; private set; }
        [field: SerializeField] public AudioClip CaptureSound { get; private set; }
    }
}
