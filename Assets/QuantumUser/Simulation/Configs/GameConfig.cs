using UnityEngine;

namespace Quantum
{
    using Photon.Deterministic;

    public class GameConfig : AssetObject
    {
        [SerializeField] private FP _maxEnemiesCount;
        [SerializeField] private FP _maxEnemiesOnAttack;
        [SerializeField] private FPVector2 _mapSize;
        [SerializeField] private FPVector3 _playerSpawnPoint;

        public FP MaxEnemiesCount => _maxEnemiesCount;
        public FP MaxEnemiesOnAttack => _maxEnemiesOnAttack;
        public FPVector2 MapExtends => _mapExtends;
        public FPVector3 PlayerSpawnPoint => _playerSpawnPoint;

        private FPVector2 _mapExtends;
        
        public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
        {
            base.Loaded(resourceManager, allocator);

            _mapExtends = _mapSize / 2;
        }
    }
}
