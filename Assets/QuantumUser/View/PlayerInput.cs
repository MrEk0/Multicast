using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private void OnEnable()
    {
        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    }

    /// <summary>
    /// Set an empty input when polled by the simulation.
    /// </summary>
    /// <param name="callback"></param>
    public void PollInput(CallbackPollInput callback)
    {
        Quantum.Input i = new Quantum.Input();

        var x = UnityEngine.Input.GetAxis("Horizontal");
        var y = UnityEngine.Input.GetAxis("Vertical");

        i.Direction = new FPVector2(x.ToFP(), y.ToFP());
        
        callback.SetInput(i, DeterministicInputFlags.Repeatable);
    }
}
