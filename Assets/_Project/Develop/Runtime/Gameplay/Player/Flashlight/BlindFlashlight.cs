using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Flashlight
{
    public class BlindFlashlight : MonoBehaviour
    {
        [SerializeField] private Light _light;

        [Header("Настройки обзора")]
        [SerializeField] private float _lightRange = 10f;
        [SerializeField] private float _lightAngle = 60f;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private LayerMask _targetMask;

        [Header("Настройки визуализации")]
        [SerializeField] private bool _isGizmoVisible = true;
        
        private readonly List<BlindablePart> _targetsInSight = new();
        private readonly Collider[] _targetsBuffer = new Collider[4];

        private void Awake()
        {
            Assert.IsNotNull(_light);
        }

        private void OnValidate()
        {
            UpdateLightSettings();
        }

        private void UpdateLightSettings()
        {
            if (_light == null)
                return;

            _light.type = LightType.Spot;
            _light.spotAngle = _lightAngle;
            _light.range = _lightRange;
            _light.innerSpotAngle = Mathf.Clamp(_lightAngle * 0.8f, 1f, _lightAngle);
        }

        private void Update()
        {
            _light.transform.rotation = transform.rotation;

            if (!_light.enabled)
                return;

            CheckTargetsInSight();

            if (_targetsInSight.Count > 0)
            {
                foreach (var consultant in _targetsInSight)
                {
                    consultant.ApplyBlind();
                }
            }
        }

        public void EnableLight() 
            => _light.enabled = true;

        public void DisableLight()
            => _light.enabled = false;

        private void CheckTargetsInSight()
        {
            _targetsInSight.Clear();

            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _lightRange,
                _targetsBuffer,
                _targetMask
            );

            for (int i = 0; i < count; i++)
            {
                Collider target = _targetsBuffer[i];

                if (!target.TryGetComponent(out BlindablePart consultant))
                    continue;

                if (!consultant.enabled || !consultant.gameObject.activeInHierarchy)
                    continue;

                Transform potentialTarget = consultant.transform;
                Vector3 directionToTarget = (potentialTarget.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget <= _lightAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, potentialTarget.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                    {
                        _targetsInSight.Add(consultant);
                    }
                }
            }
        }

        #region Gizmos
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_isGizmoVisible)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _lightRange);

            Vector3 viewAngleA = DirectionFromAngle(-_lightAngle / 2);
            Vector3 viewAngleB = DirectionFromAngle(_lightAngle / 2);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * _lightRange);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * _lightRange);
        }

        private Vector3 DirectionFromAngle(float angleInDegrees)
        {
            float angle = transform.eulerAngles.y + angleInDegrees;
            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        }
#endif
        #endregion
    }
}