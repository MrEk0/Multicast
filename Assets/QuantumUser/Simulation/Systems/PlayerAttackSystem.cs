using UnityEngine;

namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerAttackSystem : SystemMainThreadFilter<PlayerAttackSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform3D;
            public EntityHealth* Health;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            var config = f.FindAsset(f.RuntimeConfig.PlayerConfig);
           
            var playerFilter = f.Filter<PlayerEntityLevel, Transform3D>();
            while (playerFilter.Next(out var entity, out var playerEntityLevel, out var playerPosition))
            {
                var entityPosition = filter.Transform3D->Position;
                var level = playerEntityLevel.AttackRadiusLevel;

                if (FPVector3.Distance(playerPosition.Position, entityPosition) > config.AttackRadius.Value(level))
                    return;

                f.Signals.EntityHit(filter.Entity, config.Damage.Value(level) * f.DeltaTime);
            }
        }
    }
}
