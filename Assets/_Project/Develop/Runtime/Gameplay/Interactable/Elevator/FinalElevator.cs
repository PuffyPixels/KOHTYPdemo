using Assets._Project.Develop.Runtime.Gameplay.Interactable.FakeTv;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using DG.Tweening;
using DyrdaDev.FirstPersonController;
using Project.Develop.Runtime.Utilities.Initializing;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Elevator
{
    public class FinalElevator : MonoBehaviour
    {
        [SerializeField]
        private ElevatorHandler _elevatorHandler;
        [SerializeField]
        private Transform _lookOutPoint;
        [SerializeField]
        private Transform _insidePoint;

        private ICoroutinesPerformer _coroutinesPerformer;
        private SceneSwitcherService _sceneSwitcher;

        FakeTvHandler _fakeTv;

        public void Init(FakeTvHandler fakeTvHandler, ICoroutinesPerformer coroutinesPerformer, 
            SceneSwitcherService sceneSwitcher)
        {
            _fakeTv = fakeTvHandler;
            _coroutinesPerformer = coroutinesPerformer;
            _sceneSwitcher = sceneSwitcher;
            _fakeTv.Taken += OnTvTaken;
            _elevatorHandler.PlayerEntered += OnPlayerEntered;
        }


        private IEnumerator FinalRoutine(Transform player)
        {
            yield return player.DOLookAt(_insidePoint.position, Settings.Settings.ELEVATOR_LOOK_TIME).WaitForCompletion();
            yield return player.DOMove(_insidePoint.position, Settings.Settings.ELEVATOR_MOVING_TIME).WaitForCompletion();
            yield return player.DOLookAt(_lookOutPoint.position, Settings.Settings.ELEVATOR_LOOK_TIME).WaitForCompletion();
            yield return _elevatorHandler.CloseDoors();
            Hero hero = GameObject.FindFirstObjectByType<Hero>();

            if (hero != null && hero.TryGetComponent(out Canvas canvas))
            {
                GameObject textObj = new GameObject("ToBeContinued", typeof(RectTransform), typeof(TMP_Text));
                textObj.transform.SetParent(canvas.transform, false);
                RectTransform rectTransform = textObj.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(400, 100);

                TMP_Text text = textObj.GetComponent<TMP_Text>();
                text.text = "Продолжение следует...";
                text.fontSize = 40;
                text.alignment = TextAlignmentOptions.Center;
                text.color = Color.white;
            }

            //BaseSceneEntitiesInitializer.ReloadGame();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _coroutinesPerformer.StartPerform(_sceneSwitcher.ProcessSwitchTo(Scenes.MainMenu));
        }

        private void OnTvTaken()
        {
            _fakeTv.Taken -= OnTvTaken;
            StartCoroutine(_elevatorHandler.OpenDoors());
        }

        private void OnPlayerEntered()
        {
            _elevatorHandler.PlayerEntered -= OnPlayerEntered;
            var input = GameObject.FindFirstObjectByType<InputActionBasedFirstPersonControllerInput>();

            if (input != null)
            {
                input.enabled = false;
                StartCoroutine(FinalRoutine(input.transform));
            }
        }

        private void OnDestroy()
        {
            _fakeTv.Taken -= OnTvTaken;
            _elevatorHandler.PlayerEntered -= OnPlayerEntered;
        }
    }
}
