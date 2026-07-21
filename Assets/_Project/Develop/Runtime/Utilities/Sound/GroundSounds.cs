using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Sound
{
    //Is required on all surfaces player can walk on. Also requires a Collider to be registered.
    //Make sure that collider is present
    
    public class GroundSounds : MonoBehaviour
    {
        [SerializeField] 
        private AudioClip[] _stepSounds;
        [SerializeField]
        private float _stepVolume = 1f;
        [SerializeField]
        private Vector2 _pitchRange = Vector2.one;

        private void Awake()
        {
            foreach(AudioClip stepSound in _stepSounds)
            {
                if(stepSound == null)
                {
                    Debug.LogError("The " + gameObject.name + " has ground sound == null");
                }
            }
        }

        public StepSoundData GetStepSound()
        {
            return new StepSoundData
            {
                Clip = _stepSounds[UnityEngine.Random.Range(0, _stepSounds.Length)],
                Volume = _stepVolume,
                PitchRange = _pitchRange,
            };
        }
    }

    [Serializable]
    public struct StepSoundData
    {
        public AudioClip Clip;
        public float Volume;
        public Vector2 PitchRange;
    }
}