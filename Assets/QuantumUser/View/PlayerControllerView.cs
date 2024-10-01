using UniRx;
using UnityEngine;

namespace Quantum
{
    public class PlayerControllerView : QuantumEntityViewComponent
    {
        private int _killCount;
        
        private readonly CompositeDisposable _disposable = new();

        public override void OnActivate(Frame frame)
        {
            MessageBroker.Default.Receive<UpgradeButtonClickSignal>().Subscribe(_ =>
            {
                UpgradePlayer(frame);
            }).AddTo(_disposable);
            MessageBroker.Default.Receive<EntityDeathSignal>().Subscribe(OnEntityDead).AddTo(_disposable);
            
            MessageBroker.Default.Publish(new EnemyKillsChange(_killCount));
        }

        public override void OnDeactivate()
        {
            _disposable.Dispose();
        }

        private void UpgradePlayer(Frame frame)
        {
            var config = frame.FindAsset(frame.RuntimeConfig.PlayerConfig);

            var shipsFilter = frame.Filter<PlayerEntityLevel>();
            while (shipsFilter.Next(out var entity, out var entityLevel))
            {
                var rndChance = Random.Range(0f, 1f).ToFP();

                entityLevel.DamageLevel =
                    rndChance > config.AttackRadius.UpgradeChance && rndChance <= config.Velocity.UpgradeChance ? entityLevel.DamageLevel + 1 : entityLevel.DamageLevel;
                entityLevel.AttackRadiusLevel = rndChance <= config.AttackRadius.UpgradeChance ? entityLevel.AttackRadiusLevel + 1 : entityLevel.AttackRadiusLevel;
                entityLevel.VelocityLevel = rndChance > config.Velocity.UpgradeChance ? entityLevel.VelocityLevel + 1 : entityLevel.VelocityLevel;
                frame.Set(entity, entityLevel);

                frame.Signals.PlayerVelocityUpgrade(config.Velocity.Value(entityLevel.VelocityLevel));

                MessageBroker.Default.Publish(new PlayerLevelUpSignal(
                        config.Damage.Value(entityLevel.DamageLevel).AsFloat, 
                        config.AttackRadius.Value(entityLevel.AttackRadiusLevel).AsFloat,
                        config.Velocity.Value(entityLevel.VelocityLevel).AsFloat));
            }
        }

        private void OnEntityDead(EntityDeathSignal signal)
        {
            _killCount++;
            MessageBroker.Default.Publish(new EnemyKillsChange(_killCount));
        }
    }
}
