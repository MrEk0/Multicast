using Photon.Deterministic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quantum
{
    public class PlayerInput : MonoBehaviour
    {
        private InputActions.PlayerActions _player;
        private FPVector2 _direction;
        private DispatcherSubscription _subscription;

        private void Awake()
        {
            var inputSystemActions = new InputActions();

            _player = inputSystemActions.Player;
            _player.Enable();
        }

        private void OnEnable()
        {
            _subscription = QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));

            _player.Move.started += OnMoveStarted;
            _player.Move.canceled += OnMoveCancelled;
        }

        private void OnDisable()
        {
            _player.Move.performed -= OnMoveStarted;
            _player.Move.canceled -= OnMoveCancelled;

            QuantumCallback.Unsubscribe(_subscription);
        }

        private void OnMoveStarted(InputAction.CallbackContext value)
        {
            var inputMovement = value.ReadValue<Vector2>();

            _direction = new FPVector2(inputMovement.x.ToFP(), inputMovement.y.ToFP());
        }

        private void OnMoveCancelled(InputAction.CallbackContext obj)
        {
            _direction = new FPVector2();
        }

        private void PollInput(CallbackPollInput callback)
        {
            callback.SetInput(new Input { Direction = _direction }, DeterministicInputFlags.Repeatable);
        }
    }
}