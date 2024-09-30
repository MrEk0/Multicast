namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerAttackSystem : SystemSignalsOnly, ISignalOnTrigger3D
    {
        public void OnTrigger3D(Frame f, TriggerInfo3D info)
        {
            if (f.Unsafe.TryGetPointer<EntityHealth>(info.Other, out _) && f.Unsafe.TryGetPointer<PlayerLink>(info.Entity, out _))
            {
                var config = f.FindAsset(f.RuntimeConfig.PlayerConfig);
                
                var playerPos = f.Has<Transform3D>(info.Entity) ? f.Get<Transform3D>(info.Entity).Position : FPVector3.Zero;
                var entityPosition = f.Has<Transform3D>(info.Other) ? f.Get<Transform3D>(info.Other).Position : FPVector3.Zero;
                
                var level = f.Get<EntityLevel>(info.Entity).Level;
                
                if (FPVector3.Distance(playerPos, entityPosition) > config.AttackRadius.Value(level))
                    return;
                
                f.Signals.EntityHit(info.Other, config.Damage.Value(level) * f.DeltaTime);
            }
        }
    }
}
