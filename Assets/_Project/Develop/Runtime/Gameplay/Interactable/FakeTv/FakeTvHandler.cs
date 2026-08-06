using Assets._Project.Develop.Runtime.Gameplay.Interactable.ItemStorage;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.FakeTv
{
    public class FakeTvHandler : Selectable
    {
        [SerializeField]
        private ItemsCollectionHandler _itemsCollectionHandler;
        [SerializeField]
        private Collider _collider;
        [SerializeField]
        private Renderer _tv;
        [SerializeField]
        private Vector3 _offset;

        public event Action Taken;

        public override string InteractionDescription => Settings.Settings.FAKE_TV_INTERACTION_DESCRIPTION;

        private void OnCollected(bool isCorrect)
        {
            _collider.enabled = isCorrect;
            _tv.enabled = isCorrect;
        }

        public override void Interact()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            transform.parent = player.transform;
            transform.localPosition = _offset;
            Destroy(_collider);
            Destroy(this);
            Taken?.Invoke();
        }

        private void OnEnable()
        {
            _itemsCollectionHandler.Collected += OnCollected;
        }

        private void OnDisable()
        {
            _itemsCollectionHandler.Collected -= OnCollected;
        }
    }
}
