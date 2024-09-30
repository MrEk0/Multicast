using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private InputActions.PlayerActions _player;
    private FPVector2 _direction;

    private void Awake()
    {
        var inputSystemActions = new InputActions();

        _player = inputSystemActions.Player;
        _player.Enable();
    }

    private void OnEnable()
    {
        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    
        _player.Move.started += OnMoveStarted;
        _player.Move.canceled += OnMoveCancelled;
    }

    private void OnDisable()
    {
        _player.Move.performed -= OnMoveStarted;
        _player.Move.canceled -= OnMoveCancelled;
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
        var i = new Quantum.Input
        {
            Direction = _direction
        };
    
        callback.SetInput(i, DeterministicInputFlags.Repeatable);
    }
}
