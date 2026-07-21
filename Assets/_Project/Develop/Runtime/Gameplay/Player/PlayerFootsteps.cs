using Assets._Project.Develop.Runtime.Utilities.Sound;
using DyrdaDev.FirstPersonController;
using UniRx;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Player
{
    [RequireComponent(typeof(FirstPersonController))]
    public class PlayerFootsteps : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private FirstPersonController _controller;

        [Header("Ground Detection")]
        [SerializeField] private LayerMask _groundMask = ~0;
        [SerializeField] private float _rayDistance = 1.5f;

        [Header("Default Sound")]
        [SerializeField] private StepSoundData _defaultSoundData;

        private SoundsManager _soundsManager;

        private readonly RaycastHit[] _hits = new RaycastHit[1];

        private void Start()
        {
            _controller.Stepped.Subscribe(_ => PlayFootstep()).AddTo(this);
        }

        public void Init(SoundsManager soundsManager)
        {
            _soundsManager = soundsManager;
        }

        private void PlayFootstep()
        {
            Vector3 origin = _characterController.bounds.min + Vector3.up * 0.05f;
            int hitCount = Physics.RaycastNonAlloc(
                origin,Vector3.down, _hits, _rayDistance, _groundMask, QueryTriggerInteraction.Ignore);

            if (hitCount > 0 && _hits[0].collider.TryGetComponent(out GroundSounds groundSounds))
            {
                StepSoundData sound = groundSounds.GetStepSound();
                _soundsManager.PlaySound(sound.Clip, lowestPitch: sound.PitchRange.x,
                    highestPitch: sound.PitchRange.y, spawnPosition: transform,
                    volume: sound.Volume);
                return;
            }

            _soundsManager.PlaySound(_defaultSoundData.Clip, lowestPitch: _defaultSoundData.PitchRange.x,
                highestPitch: _defaultSoundData.PitchRange.y, spawnPosition: transform,
                volume: _defaultSoundData.Volume);
        }
    }
}