namespace Quantum
{
    using UnityEngine;

    public class SceneViewContext : QuantumMonoBehaviour, IQuantumViewContext
    {
        [SerializeField] private Camera _camera;

        public Camera CharacterCamera => _camera;
    }
}
