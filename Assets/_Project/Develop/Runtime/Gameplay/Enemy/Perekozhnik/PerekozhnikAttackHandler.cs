using Assets._Project.Develop.Runtime.Gameplay.Interactable.Elevator;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using DG.Tweening;
using DyrdaDev.FirstPersonController;
using Project.Develop.Runtime.Utilities.Initializing;
using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Perekozhnik
{
    public class PerekozhnikAttackHandler : IDisposable
    {
        private ElevatorHandler _elevator;
        private readonly IDisposable _faceSub;
        private readonly IObservable<Collider> _awayObserver;
        private IDisposable _awaySub;
        private Transform _animatedBone;
        private ICoroutinesPerformer _coroutinesPerformer;

        private enum Phase
        {
            Call,
            Enter,
            Move,
            Exit
        }

        private Phase _phase = Phase.Call;

        public PerekozhnikAttackHandler(ICoroutinesPerformer coroutinesPerformer, ElevatorHandler elevator, Collider faceTrigger, Collider awayTrigger, Transform animatedBone)
        {
            _elevator = elevator;
            _animatedBone = animatedBone;
            _coroutinesPerformer = coroutinesPerformer;
            _elevator.PlayerEntered += OnPlayerEntered;
            _elevator.PlayerExited += OnPlayerExited;
            _elevator.DoorsOpened += OnDoorOpened;

            _faceSub = faceTrigger.OnTriggerEnterAsObservable().
                Where(x => x.CompareTag("Player")).
                Subscribe(_ => AttackPlayer());

            _awayObserver = awayTrigger.OnTriggerEnterAsObservable().
                          Where(x => x.CompareTag("Player"));
        }

        public void Dispose()
        {
            _elevator.PlayerEntered -= OnPlayerEntered;
            _elevator.PlayerExited -= OnPlayerExited;
            _elevator.DoorsOpened -= OnDoorOpened;
            _faceSub?.Dispose();
            _awaySub?.Dispose();
        }


        private void OnPlayerEntered()
        {
            if (_phase != Phase.Enter)
            {
                AttackPlayer();
                return;
            }

            _awaySub?.Dispose();
            _phase = Phase.Move;
        }

        private void OnPlayerExited()
        {
            if (_phase != Phase.Exit)
            {
                AttackPlayer();
                return;
            }
        }

        private void OnDoorOpened()
        {
            if (_phase == Phase.Call)
            {
                _awaySub = _awayObserver.Subscribe(_ => AttackPlayer());
                _phase = Phase.Enter;
            }
            else
                _phase = Phase.Exit;
        }

        private void AttackPlayer()
        {
            Debug.Log("Attack player!");
            _coroutinesPerformer.StartPerform(AttackPlayerRoutine());
        }

        private IEnumerator AttackPlayerRoutine()
        {
            var input = GameObject.FindFirstObjectByType<InputActionBasedFirstPersonControllerInput>();

            if (input == null)
                yield break;

            input.enabled = false;
            Transform player = input.transform;
            yield return player.DOLookAt(_animatedBone.position, Settings.Settings.PEREKOZHNIK_TAKE_ATTENTION_TIME).WaitForCompletion();
            yield return _animatedBone.DOLookAt(player.position, Settings.Settings.PEREKOZHNIK_ROTATION_TIME).WaitForCompletion();
            Vector3 directionToPlayer = (player.position - _animatedBone.position).normalized;
            Vector3 targetPosition = player.position - (directionToPlayer * Settings.Settings.PEREKOZHNIK_STOPPING_DISTANCE);
            yield return _animatedBone.DOMove(targetPosition, Settings.Settings.PEREKOZHNIK_MOVE_TIME).SetEase(Ease.OutCubic).WaitForCompletion();

            // Temp. TODO: Common code for game over
            BaseSceneEntitiesInitializer.ReloadGame();
        }
    }
}