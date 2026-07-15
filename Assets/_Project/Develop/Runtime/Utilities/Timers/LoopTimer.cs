using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Timers
{
    public class LoopTimer : IDisposable
    {
        public event Action Tick;

        ICoroutinesPerformer _coroutinePerformer;
        private Coroutine _cooldownProcess;
        private WaitForSeconds _delay;
        public bool IsActive { get; private set; } = false;

        public LoopTimer(float tickTime, ICoroutinesPerformer coroutinePerformer)
        {
            _delay = new WaitForSeconds(tickTime);
            _coroutinePerformer = coroutinePerformer;
        }

        public void Stop()
        {
            IsActive = false;

            if (_cooldownProcess != null)
            {
                _coroutinePerformer.StopPerform(_cooldownProcess);
                _cooldownProcess = null;
            }
        }

        public void Start()
        {
            Stop();

            IsActive = true;

            _cooldownProcess = _coroutinePerformer.StartPerform(CooldownProcess());
        }

        private IEnumerator CooldownProcess()
        {
            while (IsActive)
            {
                yield return _delay;
                Tick?.Invoke();
            }
        }

        public void Dispose()
        {
            Stop();

            _coroutinePerformer = null;
        }
    }
}
