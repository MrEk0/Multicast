using Photon.Deterministic;
using Quantum.Collections;
using Quantum.Core;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerAttackSystem : SystemSignalsOnly, ISignalOnTrigger3D, ISignalOnTriggerEnter3D, ISignalOnTriggerExit3D, ISignalOnComponentAdded<AttackTargets>, ISignalOnComponentRemoved<AttackTargets>, ISignalEntityDied
    {
        public void OnTrigger3D(Frame f, TriggerInfo3D info)
        {
            if (!f.Unsafe.TryGetPointer<EntityHealth>(info.Other, out var health))
                return;

            var attackTargets = f.Get<AttackTargets>(info.Entity);
            var list = f.ResolveList(attackTargets.Enemies);
            if (!list.Contains(info.Other))
                return;

            var playerConfig = f.FindAsset(f.RuntimeConfig.PlayerConfig);

            var level = f.Get<PlayerStats>(info.Entity).DamageLevel;
            f.Signals.EntityHit(info.Other, info.Entity, playerConfig.Damage.Value(level) * f.DeltaTime);
        }

        public void OnTriggerEnter3D(Frame f, TriggerInfo3D info)
        {
            var gameConfig = f.FindAsset(f.RuntimeConfig.GameConfig);

            if (!f.Unsafe.TryGetPointer<AttackTargets>(info.Entity, out var attackTargets))
                return;

            var list = f.ResolveList(attackTargets->Enemies);
            if (list.Contains(info.Other))
                return;

            if (list.Count < gameConfig.MaxEnemiesOnAttack)
            {
                list.Add(info.Other);
            }
            else
            {
                var playerPosition = f.Get<Transform3D>(info.Entity).Position;
                var otherPosition = f.Get<Transform3D>(info.Other).Position;
                var maxDistance = GetMaxDistanceEntity(f, list, playerPosition, out var entity);

                if (FPVector3.Distance(otherPosition, playerPosition) >= maxDistance)
                    return;

                list.Remove(entity);
                list.Add(info.Other);
            }
        }

        public void OnTriggerExit3D(Frame f, ExitInfo3D info)
        {
            if (!f.Unsafe.TryGetPointer<AttackTargets>(info.Entity, out var attackTargets))
                return;

            var list = f.ResolveList(attackTargets->Enemies);
            list.Remove(info.Other);
        }

        public void OnRemoved(Frame f, EntityRef entity, AttackTargets* component)
        {
            f.FreeList(component->Enemies);
            component->Enemies = default;
        }

        public void OnAdded(Frame f, EntityRef entity, AttackTargets* component)
        {
            component->Enemies = f.AllocateList<EntityRef>();
        }

        private FP GetMaxDistanceEntity(FrameBase f, QList<EntityRef> list, FPVector3 playerPosition,
            out EntityRef entity)
        {
            var max = FP._0;
            entity = new EntityRef();

            for (var i = 0; i < list.Count; i++)
            {
                var distance = FPVector3.Distance(playerPosition, f.Get<Transform3D>(list[i]).Position);
                if (distance <= max)
                    continue;

                max = distance;
                entity = list[i];
            }

            return max;
        }

        public void EntityDied(Frame f, EntityRef deadEntity, EntityRef killer)
        {
            var attackTargets = f.Get<AttackTargets>(killer);
            var list = f.ResolveList(attackTargets.Enemies);
            list.Remove(deadEntity);
        }
    }
}
