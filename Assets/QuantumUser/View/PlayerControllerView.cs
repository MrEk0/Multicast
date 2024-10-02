using UniRx;
using UnityEngine;

namespace Quantum
{
    public class PlayerControllerView : QuantumEntityViewComponent
    {
        private int _killCount;
        
        private readonly CompositeDisposable _disposable = new();
        private DispatcherSubscription _subscription;

        public override void OnActivate(Frame frame)
        {
            _subscription = QuantumEvent.Subscribe(listener: this, handler: (EventEntityDied e) => OnEntityDead(e));
            
            MessageBroker.Default.Receive<UpgradeButtonClickSignal>().Subscribe(_ =>
            {
                UpgradePlayer(frame);
            }).AddTo(_disposable);

            MessageBroker.Default.Publish(new EnemyKillsChange(_killCount));
        }

        public override void OnDeactivate()
        {
            QuantumEvent.Unsubscribe(_subscription);
            
            _disposable.Dispose();
        }

        private void UpgradePlayer(Frame frame)
        {
            var config = frame.FindAsset(frame.RuntimeConfig.PlayerConfig);

            var entityLevel = frame.Get<PlayerEntityLevel>(_entityView.EntityRef);
            var position = frame.Get<Transform3D>(_entityView.EntityRef).Position;

            var rndChance = Random.Range(0f, 1f).ToFP();

            entityLevel.DamageLevel = rndChance > config.AttackRadius.UpgradeChance && rndChance <= config.Velocity.UpgradeChance ? entityLevel.DamageLevel + 1 : entityLevel.DamageLevel;
            entityLevel.AttackRadiusLevel = rndChance <= config.AttackRadius.UpgradeChance ? entityLevel.AttackRadiusLevel + 1 : entityLevel.AttackRadiusLevel;
            entityLevel.VelocityLevel = rndChance > config.Velocity.UpgradeChance ? entityLevel.VelocityLevel + 1 : entityLevel.VelocityLevel;
            
            frame.Set(_entityView.EntityRef, entityLevel);

            frame.Signals.PlayerVelocityUpgrade(config.Velocity.Value(entityLevel.VelocityLevel));
            //frame.Signals.PlayerAttackRadiusUpgrade(_entityView.EntityRef, position);

            MessageBroker.Default.Publish(new PlayerLevelUpSignal(config.Damage.Value(entityLevel.DamageLevel).AsFloat, config.AttackRadius.Value(entityLevel.AttackRadiusLevel).AsFloat, config.Velocity.Value(entityLevel.VelocityLevel).AsFloat));
        }

        private void OnEntityDead(EventEntityDied e)
        {
            _killCount++;
            MessageBroker.Default.Publish(new EnemyKillsChange(_killCount));

            var attackTargets = VerifiedFrame.Get<AttackTargets>(_entityView.EntityRef);
            var list = e.frame.ResolveList(attackTargets.Enemies);
            list.Remove(e.entity);
        }
    }
}
