using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Watch
{
    public class HeroWatchPresenter : MonoBehaviour
    {
        [SerializeField] private HeroWatch _heroWatch;
        [SerializeField] private DyrdaDev.FirstPersonController.FirstPersonControllerInput _input;


        private void Awake()
        {
            Assert.IsNotNull(_heroWatch);
            Assert.IsNotNull(_input);
        }

        private void Start()
        {
            _input.Watch
                .DistinctUntilChanged()
                .Subscribe(OnWatchStateChanged)
                .AddTo(this);
        }

        private void OnWatchStateChanged(bool opened)
        {
            if (opened)
                _heroWatch.Open();
            else
                _heroWatch.Close();
        }
    }
}