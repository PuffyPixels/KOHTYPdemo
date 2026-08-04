using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy
{
    public class FieldOfView : MonoBehaviour
    {
        [SerializeField] private ConsultantFacade _consultantFacade;

        [Header("Настройки обзора")]
        [SerializeField] private float _viewRadius = 10f;
        [SerializeField] private float _viewAngle = 60f;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private LayerMask _targetMask;

        [Header("Настройки обнаружения")]
        [SerializeField] private float _detectionSpeed = 0.2f;
        [SerializeField] private float _detectionDecaySpeed = 0.1f;

        [SerializeField] private bool _isGizmoVisible = true;

        private void Awake()
        {
            Assert.IsNotNull(_consultantFacade);
        }

        private void Update()
        {
            _consultantFacade.DetectPlayer(CheckForTarget());

            if (_consultantFacade.Target != null)
            {
                if (_consultantFacade.DetectionProgress < 1f)
                    _consultantFacade.DetectionProgress += _detectionSpeed * Time.deltaTime;
            }
            else
            {
                if (_consultantFacade.DetectionProgress > 0f)
                    _consultantFacade.DetectionProgress -= _detectionDecaySpeed * Time.deltaTime;
            }
        }

        private HeroStress CheckForTarget()
        {
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);

            foreach (Collider targetCollider in targetsInViewRadius)
            {
                if (!targetCollider.TryGetComponent(out HeroStress hero))
                    continue;

                Transform potentialTarget = hero.transform;
                Vector3 directionToTarget = (potentialTarget.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget <= _viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, potentialTarget.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                    {
                        return hero;
                    }
                }
            }

            return null;
        }

        #region Gizmos
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_isGizmoVisible)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _viewRadius);

            Vector3 viewAngleA = DirectionFromAngle(-_viewAngle / 2);
            Vector3 viewAngleB = DirectionFromAngle(_viewAngle / 2);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * _viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * _viewRadius);

            if (_consultantFacade.Target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _consultantFacade.Target.transform.position);
            }
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
