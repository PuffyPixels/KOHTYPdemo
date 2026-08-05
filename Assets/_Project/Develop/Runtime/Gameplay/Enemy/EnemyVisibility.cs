using Assets._Project.Develop.Runtime.Gameplay.Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy
{
    public class EnemyVisibility : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _boundsRenderer;
        [SerializeField] private LayerMask _obstacleMask;

        private Camera _camera;
        private HeroStress _heroStress;

        private string _name;
        private bool _isInited = false;

        private Vector3[] _boundsPoints;

        public void Init(Camera camera)
        {
            _camera = camera;

            if (Application.isPlaying)
            {
                Material mat = _boundsRenderer.material;
                Color color = mat.color;
                color.a = 0f;
                mat.color = color;
            }

            if (_camera != null)
            {
                _heroStress = _camera.GetComponentInParent<HeroStress>();

                if (_heroStress == null)
                    _heroStress = _camera.GetComponent<HeroStress>();

                if (_heroStress == null)
                    _heroStress = _camera.GetComponentInChildren<HeroStress>();
            }

            _name = transform.position.ToString();

            Assert.IsNotNull(_boundsRenderer);
            Assert.IsNotNull(_camera);
            Assert.IsNotNull(_heroStress);

            _isInited = true;
        }

        private void Update()
        {
            if (!_isInited)
                return;

            bool isVisible = IsBoundsVisible();

            if (isVisible)
            {
                float distance = Vector3.Distance(transform.position, _camera.transform.position);
                float stressValue = GetStressByDistance(distance);

                _heroStress.AddStressSource(_name, stressValue);
            }
            else
            {
                _heroStress.RemoveStressSource(_name);
            }
        }

        private bool IsBoundsVisible()
        {
            Bounds bounds = _boundsRenderer.bounds;
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_camera);

            if (!GeometryUtility.TestPlanesAABB(frustumPlanes, bounds))
            {
                return false;
            }
                

            return HasLineOfSight(bounds);
        }

        private bool HasLineOfSight(Bounds bounds)
        {
            Vector3 cameraPos = _camera.transform.position;

            Vector3[] points = GetBoundsPoints(bounds);

            foreach (Vector3 point in points)
            {
                Vector3 direction = point - cameraPos;
                float distance = direction.magnitude;

                if (Physics.Raycast(cameraPos, direction.normalized, out RaycastHit hit, distance, _obstacleMask))
                {
                    if (!hit.transform.IsChildOf(transform.root))
                        continue;
                }

                return true;
            }

            return false;
        }

        private Vector3[] GetBoundsPoints(Bounds bounds)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            return new Vector3[]
            {
                center,
                center + new Vector3( extents.x,  extents.y,  extents.z),
                center + new Vector3(-extents.x,  extents.y,  extents.z),
                center + new Vector3( extents.x, -extents.y,  extents.z),
                center + new Vector3( extents.x,  extents.y, -extents.z),
                center + new Vector3(-extents.x, -extents.y,  extents.z),
                center + new Vector3(-extents.x,  extents.y, -extents.z),
                center + new Vector3( extents.x, -extents.y, -extents.z),
                center + new Vector3(-extents.x, -extents.y, -extents.z),
            };
        }

        private float GetStressByDistance(float distance)
        {
            if (distance >= 20f)
                return 0f;
            else if (distance >= 15f)
                return 1f;
            else if (distance >= 10f)
                return 2f;
            else if (distance >= 5f)
                return 3f;
            else
                return 4f;
        }
    }
}