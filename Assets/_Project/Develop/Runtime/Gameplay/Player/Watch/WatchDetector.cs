using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using NUnit.Framework;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Watch
{
    public class WatchDetector : IDisposable
    {
        private readonly Transform _hero;
        private readonly LayerMask _enemyMask;
        private readonly float _radius;
        private readonly AnimationCurve _signalCurve;
        private Inventory.Inventory _inventory;
        private float _inventoryIncorrectPercent;
        private List<string> _correctItems;

        private readonly Collider[] _results = new Collider[16];

        public bool HasEnemyNearby { get; private set; }

        public event Action<WatchSignal, bool> SignalChanged;

        public WatchDetector(
            Transform hero,
            LayerMask enemyMask,
            float radius,
            AnimationCurve signalCurve, 
            Inventory.Inventory inventory,
            List<string> correctItems)
        {
            _hero = hero;
            _enemyMask = enemyMask;
            _radius = radius;
            _signalCurve = signalCurve;
            _correctItems = correctItems;
            _inventory = inventory;
            _inventory.ItemAdded += OnItemsChanged;
            _inventory.ItemDropped += OnItemsChanged;
            _inventory.ItemRemoved += OnItemsChanged;
        }

        public void Tick()
        {
            if (_inventoryIncorrectPercent > 0.1f)
            {
                SignalChanged?.Invoke(
                    new WatchSignal(_signalCurve.Evaluate(_inventoryIncorrectPercent), WatchSignalSource.Item),
                    true);

                return;
            }

            int count = Physics.OverlapSphereNonAlloc(
                _hero.position,
                _radius,
                _results,
                _enemyMask);

            HasEnemyNearby = count > 0;

            if (count == 0)
            {
                SignalChanged?.Invoke(
                    new WatchSignal(0f, WatchSignalSource.None),
                    false);

                return;
            }

            float nearestDistance = float.MaxValue;
            int enemiesCount = 0;

            for (int i = 0; i < count; i++)
            {
                Collider collider = _results[i];

                if (!collider.TryGetComponent(out ConsultantFacade consultant))
                    continue;

                enemiesCount++;

                float distance = Vector3.Distance(
                    _hero.position,
                    consultant.transform.position);

                if (distance < nearestDistance)
                    nearestDistance = distance;
            }

            if (enemiesCount == 0)
            {
                HasEnemyNearby = false;

                SignalChanged?.Invoke(
                    new WatchSignal(0f, WatchSignalSource.None),
                    false);

                return;
            }

            float normalizedDistance = 1f - nearestDistance / _radius;
            normalizedDistance = Mathf.Clamp01(normalizedDistance);

            float strength = _signalCurve.Evaluate(normalizedDistance);

            SignalChanged?.Invoke(
                new WatchSignal(
                    strength,
                    WatchSignalSource.Enemy),
                enemiesCount > 1);
        }

        public void Dispose()
        {
            SignalChanged = null;
            _inventory.ItemAdded -= OnItemsChanged;
            _inventory.ItemDropped -= OnItemsChanged;
            _inventory.ItemRemoved -= OnItemsChanged;
        }

        private void OnItemsChanged(Inventory.Inventory _, InventoryItem __)
        {
            int incorrectNumber = 0;

            if (_inventory.Items == null || _inventory.Items.Count == 0)
                return;

            foreach (var item in _inventory.Items)
            {
                if (!_correctItems.Contains(item.Name))
                    incorrectNumber++;
            }

            _inventoryIncorrectPercent = (float)incorrectNumber / (float)_inventory.Items.Count;
        }
    }
}