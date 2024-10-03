using System.Collections.Generic;
using Photon.Deterministic;
using UniRx;
using UnityEngine;

namespace Quantum
{
    public class PlayerStatsModel
    {
        public FP DamageLevel { get; private set; } = FP._0;

        public FP AttackRadiusLevel { get; private set; } = FP._0;

        public FP VelocityLevel { get; private set; } = FP._0;

        private FP KillsCount { get; set; } = FP._0;

        public IReadOnlyList<EntityRef> AttackTargets => _attackTargets;

        private readonly List<EntityRef> _attackTargets = new();

        public void Upgrade(PlayerGameConfig config)
        {
            var rndChance = Random.Range(0f, 1f);

            DamageLevel = rndChance > config.AttackRadius.UpgradeChance && rndChance <= config.Velocity.UpgradeChance ? DamageLevel + 1 : DamageLevel;
            AttackRadiusLevel = rndChance <= config.AttackRadius.UpgradeChance ? AttackRadiusLevel + 1 : AttackRadiusLevel;
            VelocityLevel = rndChance > config.Velocity.UpgradeChance ? VelocityLevel + 1 : VelocityLevel;

            MessageBroker.Default.Publish(new PlayerLevelUpSignal(config.Damage.Value(DamageLevel).AsFloat, config.AttackRadius.Value(AttackRadiusLevel).AsFloat, config.Velocity.Value(VelocityLevel).AsFloat));
        }

        public void AddTarget(EntityRef entity)
        {
            _attackTargets.Add(entity);
        }

        public void RemoveTarget(EntityRef entity)
        {
            _attackTargets.Remove(entity);
        }

        public void OnEntityDied(EventEntityDied e)
        {
            KillsCount += 1;

            _attackTargets.Remove(e.entity);
            
            MessageBroker.Default.Publish(new EnemyKillsChange(KillsCount.AsInt));
        }
        
        public FP GetMaxDistanceEntity(Frame frame, FPVector3 playerPosition)
        {
            var max = FP._0;

            for (var i = 0; i < _attackTargets.Count; i++)
            {
                var distance = FPVector3.Distance(playerPosition, frame.Get<Transform3D>(_attackTargets[i]).Position);
                if (distance <= max)
                    continue;
            
                max = distance;
            }
            
            return max;
        }
    }
}
