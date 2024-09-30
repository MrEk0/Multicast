namespace Quantum
{
    using Photon.Deterministic;

    public class GameConfig : AssetObject
    {
        public FP MaxEnemiesCount;
        public FP MaxEnemiesOnAttack;
        public FPVector2 MapSize;

        public FPVector2 MapExtends => _mapExtends;

        private FPVector2 _mapExtends;
        
        public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
        {
            base.Loaded(resourceManager, allocator);

            _mapExtends = MapSize / 2;
        }
    }
}
