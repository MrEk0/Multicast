using System.Collections.Generic;
using System.Linq;
using Photon.Deterministic;
using TMPro;
using UniRx;
using UnityEngine;

namespace Quantum
{
    public class PlayerEntityView : QuantumViewComponent<SceneViewContext>
    {
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private Vector3 _cameraOffset;

        private bool _isLocal;
        private PlayerStatsModel _model;
        private PhysicsCollider3D _collider;

        private readonly CompositeDisposable _disposable = new();
        private readonly List<DispatcherSubscription> _subscriptions = new();

        public override void OnInitialize()
        {
            _model = new PlayerStatsModel();
        }

        public override void OnActivate(Frame frame)
        {
            var playerLink = frame.Get<PlayerLink>(_entityView.EntityRef);
            var playerData = frame.GetPlayerData(playerLink.Player);
            
            var config = frame.FindAsset(frame.RuntimeConfig.PlayerConfig);
            _isLocal = Game.PlayerIsLocal(playerLink.Player);

            _playerName.text = playerData.PlayerNickname;
            
            _collider = PhysicsCollider3D.Create(VerifiedFrame, Shape3D.CreateSphere(config.AttackRadius.Value(_model.AttackRadiusLevel)));
            _collider.IsTrigger = true;

            VerifiedFrame.Signals.PlayerVelocityUpgrade(config.Velocity.Value(_model.VelocityLevel));
            
            MessageBroker.Default.Receive<UpgradeButtonClickSignal>().Subscribe(OnUpgradeButtonClicked).AddTo(_disposable);
            MessageBroker.Default.Publish(new PlayerLevelUpSignal(config.Damage.Value(_model.DamageLevel).AsFloat, config.AttackRadius.Value(_model.AttackRadiusLevel).AsFloat, config.Velocity.Value(_model.VelocityLevel).AsFloat));

            _subscriptions.Add(QuantumEvent.Subscribe(listener: this, handler: (EventEntityDied e) => OnEntityKilled(e)));
            _subscriptions.Add(QuantumEvent.Subscribe(listener: this, handler: (EventOnPlayerEntityTriggerEnter e) => OnPlayerEntityTriggerEnter(e.other)));
            _subscriptions.Add(QuantumEvent.Subscribe(listener: this, handler: (EventOnPlayerEntityTriggerExit e) => OnPlayerEntityTriggerExit(e.other)));
            _subscriptions.Add(QuantumEvent.Subscribe(listener: this, handler: (EventOnPlayerEntityTriggerStay e) => OnPlayerEntityTriggerStay(e.other, e.entity)));
        }
        
        public override void OnDeactivate()
        {
            foreach (var subscription in _subscriptions)
                QuantumEvent.Unsubscribe(subscription);

            _disposable.Dispose();
        }

        public override void OnUpdateView()
        {
            if (!_isLocal)
                return;

            var tr = transform;
            ViewContext.CharacterCamera.transform.position = tr.position + _cameraOffset;
            ViewContext.CharacterCamera.transform.LookAt(tr);
        }

        private void OnEntityKilled(EventEntityDied e)
        {
            if (e.killer != _entityView.EntityRef)
                return;

            _model.ChangeKillCount(e.entity);
        }

        private void OnUpgradeButtonClicked(UpgradeButtonClickSignal signal)
        {
            var config = VerifiedFrame.FindAsset(VerifiedFrame.RuntimeConfig.PlayerConfig);
            
            _model.Upgrade(VerifiedFrame.FindAsset(VerifiedFrame.RuntimeConfig.PlayerConfig));
            
            _collider.Shape.Sphere.Radius = config.AttackRadius.Value(_model.AttackRadiusLevel);
            VerifiedFrame.Set(_entityView.EntityRef, _collider);
            
            VerifiedFrame.Signals.PlayerVelocityUpgrade(config.Velocity.Value(_model.VelocityLevel));
        }

        private void OnPlayerEntityTriggerStay(EntityRef other, EntityRef entity)
        {
            if (!VerifiedFrame.Has<EntityHealth>(other))
                return;

            if (!_model.AttackTargets.Contains(other))
                return;
            
            var playerConfig = VerifiedFrame.FindAsset(VerifiedFrame.RuntimeConfig.PlayerConfig);
            var gameConfig = VerifiedFrame.FindAsset(VerifiedFrame.RuntimeConfig.GameConfig);

            if (_model.AttackTargets.Count >= gameConfig.MaxEnemiesOnAttack)
            {
                var playerPosition = VerifiedFrame.Get<Transform3D>(entity).Position;
                var otherPosition = VerifiedFrame.Get<Transform3D>(other).Position;
                var maxDistance = _model.GetMaxDistanceEntity(VerifiedFrame, playerPosition);
             
                if (FPVector3.Distance(otherPosition, playerPosition) >= maxDistance)
                    return;
            }
            
            VerifiedFrame.Signals.EntityHit(other, entity, playerConfig.Damage.Value(_model.DamageLevel) * VerifiedFrame.DeltaTime);
        }

        private void OnPlayerEntityTriggerExit(EntityRef other)
        {
            _model.RemoveTarget(other);
        }

        private void OnPlayerEntityTriggerEnter(EntityRef other)
        {
            if (!VerifiedFrame.Has<EntityHealth>(other))
                return;
            
            if (_model.AttackTargets.Contains(other))
                return;

            _model.AddTarget(other);
        }
    }
}
