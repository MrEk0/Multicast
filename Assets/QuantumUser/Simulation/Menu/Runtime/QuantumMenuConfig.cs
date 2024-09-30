namespace Quantum.Menu
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [ScriptHelp(BackColor = ScriptHeaderBackColor.Blue)]
    [CreateAssetMenu(menuName = "Quantum/Menu/Menu Config")]
    public partial class QuantumMenuConfig : QuantumScriptableObject
    {
        [SerializeField] protected int _maxPlayers = 6;
        [SerializeField] protected bool _adaptFramerateForMobilePlatform = true;
        [SerializeField] protected List<string> _availableAppVersions = new() { "1.0" };
        [SerializeField] protected List<string> _availableRegions = new() { "asia", "eu", "sa", "us" };
        [SerializeField] protected List<PhotonMenuSceneInfo> _availableScenes = new();
        [SerializeField] protected QuantumMenuMachineId _machineId;
        [SerializeField] protected QuantumMenuPartyCodeGenerator _codeGenerator;
        
        public List<string> AvailableAppVersions => _availableAppVersions;
        public List<string> AvailableRegions => _availableRegions;
        public List<PhotonMenuSceneInfo> AvailableScenes => _availableScenes;
        public int MaxPlayerCount => _maxPlayers;
        public virtual string MachineId => _machineId?.Id;
        public QuantumMenuPartyCodeGenerator CodeGenerator => _codeGenerator;
        
        public bool AdaptFramerateForMobilePlatform => _adaptFramerateForMobilePlatform;
    }
}