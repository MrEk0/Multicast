using Cinemachine;
using UnityEngine;

namespace Quantum
{
    public class MapViewContext : MonoBehaviour, IQuantumViewContext
    {
        [SerializeField] private CinemachineVirtualCameraBase _virtualCamera;

        public CinemachineVirtualCameraBase VirtualCameraBase => _virtualCamera;
    }
}

