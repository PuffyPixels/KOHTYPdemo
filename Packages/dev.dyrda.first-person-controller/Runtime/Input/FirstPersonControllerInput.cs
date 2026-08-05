using System;
using UniRx;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    public abstract class FirstPersonControllerInput : MonoBehaviour
    {
        /// <summary>
        ///     Move axes in WASD / D-Pad style.
        ///     Interaction type: continuous axes.
        /// </summary>
        public abstract IObservable<Vector2> Move { get; }

        /// <summary>
        ///     Jump button.
        ///     Interaction type: Trigger.
        /// </summary>
        public abstract IObservable<Unit> Jump { get; }

        /// <summary>
        ///     Crouch button.
        ///     Interaction type: Toggle.
        /// </summary>
        public abstract IObservable<Unit> Crouch { get; }

        /// <summary>
        ///     Use button.
        ///     Interaction type: Trigger.
        /// </summary>
        public abstract IObservable<Unit> Use { get; }

        /// <summary>
        ///     Run button.
        ///     Interaction type: Toggle.
        /// </summary>
        public abstract ReadOnlyReactiveProperty<bool> Run { get; }

        /// <summary>
        ///     Look axes following the free look (mouse look) pattern.
        ///     Interaction type: continuous axes.
        /// </summary>
        public abstract IObservable<Vector2> Look { get; }

        /// <summary>
        ///     Crouch state (true = pressed, false = released).
        ///     Interaction type: State (not event).
        /// </summary>
        public abstract IObservable<bool> CrouchState { get; }

        /// <summary>
        ///     Watch button.
        ///     Interaction type: Toggle.
        /// </summary>
        public abstract ReadOnlyReactiveProperty<bool> Watch { get; }
    }
}