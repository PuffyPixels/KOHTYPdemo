using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    /// <summary>
    ///     Controller that handles the character controls and camera controls of the first person player.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour, ICharacterSignals
    {
        #region Character Signals

        public IObservable<Vector3> Moved => _moved;
        private Subject<Vector3> _moved;

        public ReactiveProperty<bool> IsRunning => _isRunning;
        private ReactiveProperty<bool> _isRunning;

        public IObservable<Unit> Landed => _landed;
        private Subject<Unit> _landed;

        public IObservable<Unit> Jumped => _jumped;
        private Subject<Unit> _jumped;

        public IObservable<Unit> Used => _used;
        private Subject<Unit> _used;

        public IObservable<Unit> Stepped => _stepped;
        private Subject<Unit> _stepped;

        #endregion

        #region Configuration

        [Header("References")]
        [SerializeField] private FirstPersonControllerInput firstPersonControllerInput;
        private CharacterController _characterController;
        private Camera _camera;

        [Header("Locomotion Properties")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float jumpForceMagnitude = 10f;
        [SerializeField] private float strideLength = 4f;
        [SerializeField] private float crouchDelta = 0.5f;
        public float StrideLength => strideLength;
        [SerializeField] private float stickToGroundForceMagnitude = 5f;

        [Header("Look Properties")]
        [Range(-90, 0)] [SerializeField] private float minViewAngle = -60f;
        [Range(0, 90)] [SerializeField] private float maxViewAngle = 60f;

        #endregion

        private bool _isCrouched;
        private float _normalHeight, _crouchHeight;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _camera = GetComponentInChildren<Camera>();

            _isRunning = new ReactiveProperty<bool>(false);
            _moved = new Subject<Vector3>().AddTo(this);
            _jumped = new Subject<Unit>().AddTo(this);
            _used = new Subject<Unit>().AddTo(this);
            _landed = new Subject<Unit>().AddTo(this);
            _stepped = new Subject<Unit>().AddTo(this);
            _normalHeight = _characterController.height;
            _crouchHeight = _normalHeight * crouchDelta;
        }

        private void Start()
        {
            HandleLocomotion();

            HandleSteppedCharacterSignal();

            HandleLook();

            HandleUse();
        }

        private void HandleLocomotion()
        {
            // Ensures the first frame counts as "grounded".
            _characterController.Move(-stickToGroundForceMagnitude * transform.up);

            // Create a jump latch for sync + map from events to true/false values.
            var jumpLatch = LatchObservables.Latch(this.UpdateAsObservable(), firstPersonControllerInput.Jump,  false);

            // Create a crouch latch for sync from bool stream.
            var crouchStateStream = firstPersonControllerInput.Crouch
                .Scan(false, (acc, _) => !acc);

            var crouchEveryFrame = this.UpdateAsObservable()
                .WithLatestFrom(crouchStateStream, (_, state) => state)
                .Where(state => state)
                .Select(_ => Unit.Default);

            var crouchLatch = LatchObservables.Latch(this.UpdateAsObservable(), crouchEveryFrame, false);

            // Handle move:
            firstPersonControllerInput.Move
                .Zip(jumpLatch, crouchLatch, (m, j, c) => new MoveInputData(m, j, c))
                .Where(moveInputData => moveInputData.Jump ||
                                        moveInputData.Crouch != _isCrouched ||
                                        moveInputData.Move != Vector2.zero ||
                                        _characterController.isGrounded == false)
                .Subscribe(i =>
                {
                    var wasGrounded = _characterController.isGrounded;

                    // Apply crouch.
                    if (_isCrouched != i.Crouch)
                    {
                        _isCrouched = i.Crouch;
                        _characterController.height = _isCrouched ? _crouchHeight : _normalHeight;

                        if (!_isCrouched && wasGrounded)
                        {
                            var distanceToShift = (_normalHeight - _crouchHeight) * 0.5f;
                            _characterController.enabled = false;
                            transform.position += Vector3.up * distanceToShift;
                            _characterController.enabled = true;
                        }
                    }

                    // Vertical movement:
                    var verticalVelocity = 0f;
                    // The character is ...
                    if (i.Jump && wasGrounded)
                    {
                        // ... grounded and wants to jump.
                        
                        verticalVelocity = jumpForceMagnitude;
                        _jumped.OnNext(Unit.Default);
                    }
                    else if (!wasGrounded)
                    {
                        // ... in the air.
                        
                        verticalVelocity = _characterController.velocity.y + Physics.gravity.y * Time.deltaTime * 3.0f;
                    }
                    else
                    {
                        // ... otherwise grounded.
                        
                        verticalVelocity = -Mathf.Abs(stickToGroundForceMagnitude);
                    }

                    // Horizontal movement:
                    var currentSpeed = firstPersonControllerInput.Run.Value && !_isCrouched ? runSpeed : walkSpeed;
                    var horizontalVelocity = i.Move * currentSpeed; //Calculate velocity (direction * speed).

                    // Combine horizontal and vertical movement.
                    var characterVelocity = transform.TransformVector(new Vector3(
                        horizontalVelocity.x,
                        verticalVelocity,
                        horizontalVelocity.y));

                    // Apply movement.
                    var motion = characterVelocity * Time.deltaTime;
                    _characterController.Move(motion);

                    // Set ICharacterSignals output signals related to the movement.
                    HandleLocomotionCharacterSignalsIteration(wasGrounded, _characterController.isGrounded);
                }).AddTo(this);
        }

        private void HandleLocomotionCharacterSignalsIteration(bool wasGrounded, bool isGrounded)
        {
            var tempIsRunning = false;

            if (wasGrounded && isGrounded)
            {
                // The character was grounded at the beginning and end of this frame.

                _moved.OnNext(_characterController.velocity * Time.deltaTime);

                if (_characterController.velocity.magnitude > 0)
                {
                    // The character is running if the input is active and
                    // the character is actually moving on the ground
                    tempIsRunning = firstPersonControllerInput.Run.Value;
                }
            }

            if (!wasGrounded && isGrounded)
            {
                // The character was airborne at the beginning, but grounded at the end of this frame.
                
                _landed.OnNext(Unit.Default);
            }

            _isRunning.Value = tempIsRunning;
        }

        private void HandleSteppedCharacterSignal()
        {
            // Emit stepped events:
            
            var stepDistance = 0f;
            Moved.Subscribe(w =>
            {
                stepDistance += w.magnitude;

                if (stepDistance > strideLength)
                {
                    _stepped.OnNext(Unit.Default);
                }

                stepDistance %= strideLength;
            }).AddTo(this);
        }

        private void HandleLook()
        {
            firstPersonControllerInput.Look
                .Where(v => v != Vector2.zero)
                .Subscribe(inputLook =>
                {
                    // Translate 2D mouse input into euler angle rotations.

                    // Horizontal look with rotation around the vertical axis, where + means clockwise.
                    var horizontalLook = inputLook.x * Vector3.up * Time.deltaTime;
                    transform.localRotation *= Quaternion.Euler(horizontalLook);

                    // Vertical look with rotation around the horizontal axis, where + means upwards.
                    var verticalLook = inputLook.y * Vector3.left * Time.deltaTime;
                    var newQ = _camera.transform.localRotation * Quaternion.Euler(verticalLook);
                    
                    _camera.transform.localRotation =
                        RotationTools.ClampRotationAroundXAxis(newQ, -maxViewAngle, -minViewAngle);
                }).AddTo(this);
        }

        private void HandleUse()
        {
            var useLatch = LatchObservables.Latch(this.UpdateAsObservable(), firstPersonControllerInput.Use, false);
            useLatch.Where(use => use).Subscribe(_ =>
            {
                _used.OnNext(Unit.Default);
            }).AddTo(this);
        }

        public struct MoveInputData
        {
            public readonly Vector2 Move;
            public readonly bool Jump;
            public readonly bool Crouch;

            public MoveInputData(Vector2 move, bool jump, bool crouch)
            {
                Move = move;
                Jump = jump;
                Crouch = crouch;
            }
        }
    }
}