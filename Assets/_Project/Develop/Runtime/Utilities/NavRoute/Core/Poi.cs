using Assets.NavRoute.Movement;
using UnityEngine;

namespace Assets.NavRoute.Core
{
    public class Poi : Waypoint
    {
        private const int MINIMAL_POI_PRIORITY = 0;

        private int _allPoisPriorityWithoutThis;
        private int _cachedId = -1;

        [field: SerializeField] public PoiAnimations OnPoiAnimation { get; private set; } = PoiAnimations.Idle;
        [field: SerializeField] public float TimeModificator { get; private set; } = 1f;
        [field: SerializeField, Range(MINIMAL_POI_PRIORITY, 10)] public int Priority { get; private set; } = 5;

        public int Id
        {
            get
            {
                if (_cachedId != -1)
                    return _cachedId;

                int x = (int)transform.position.x;
                int z = (int)transform.position.z;
                _cachedId = (x << 16) | (z & 0xFFFF);

                return _cachedId;
            }
        }

        public int AllPoisPriorityWithoutThis
        {
            get
            {
                if (_allPoisPriorityWithoutThis > 0)
                    return _allPoisPriorityWithoutThis;

                Debug.LogError($"{gameObject.name}: Total weight without this point is not inited");
                return 0;
            }
            private set
            {
                if (value < 0)
                {
                    Debug.LogError($"{gameObject.name}: Total weight without this point can't be: {value}");
                    return;
                }

                _allPoisPriorityWithoutThis = value;
            }
        }

        public void Init(int allPoisPriorityWithoutThis)
        {
            _ = Id;
            AllPoisPriorityWithoutThis = allPoisPriorityWithoutThis;
        }

#if UNITY_EDITOR
        private const float POI_SPHERE_RADIUS = 0.3f;
        private readonly Color POI_COLOR = Color.blue;
       
        private const float LABEL_OFFSET_Y = 3f;
        private const int LABEL_FONT_SIZE = 12;


        private GUIStyle _labelStyle;

        protected override void DrawSphere()
        {
            Gizmos.color = Priority > MINIMAL_POI_PRIORITY ? POI_COLOR : WAYPOINT_COLOR;
            Gizmos.DrawSphere(transform.position, POI_SPHERE_RADIUS);
        }

        protected override void AdditionalDraw() => DrawLabel();

        private void DrawLabel()
        {
            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle();
                _labelStyle.normal.textColor = POI_COLOR;
                _labelStyle.fontSize = LABEL_FONT_SIZE;
                _labelStyle.fontStyle = FontStyle.Bold;
                _labelStyle.alignment = TextAnchor.MiddleCenter;
            }

            Vector3 labelPos = Position + (Vector3.up * LABEL_OFFSET_Y);
            UnityEditor.Handles.Label(labelPos, gameObject.name, _labelStyle);
        }
#endif
    }
}