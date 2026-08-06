using Assets._Project.Develop.Runtime.Gameplay.Interactable.ItemStorage;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Cashier
{ 
    public class Cashier : MonoBehaviour
    {
        [SerializeField]
        private ItemsCollectionHandler _itemsHandler;
        [SerializeField]
        private Animator _animator;

        private void OnEnable()
        {
            _itemsHandler.Collected += OnCollected;
        }

        private void OnDisable()
        {
            _itemsHandler.Collected -= OnCollected;
        }

        private void OnCollected(bool isCorrect)
        {
            _animator.SetTrigger(isCorrect ? "Correct" : "Incorrect");
        }
    }
}