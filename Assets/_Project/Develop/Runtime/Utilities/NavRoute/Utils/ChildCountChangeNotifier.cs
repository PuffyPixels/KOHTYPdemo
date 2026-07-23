using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.NavRoute.Utils
{
    [ExecuteAlways]
    public class ChildCountChangeNotifier : MonoBehaviour
    {
        public Action<int> ChildCountChanged;

        private void OnTransformChildrenChanged() =>
            ChildCountChanged?.Invoke(transform.childCount);
    }
}