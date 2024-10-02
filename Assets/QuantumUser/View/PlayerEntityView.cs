using TMPro;
using UniRx;
using UnityEngine;

namespace Quantum
{
    public class PlayerEntityView : QuantumEntityViewComponent
    {
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private QuantumEntityPrototype _entityPrototype;

        private readonly CompositeDisposable _disposable = new();

        public override void OnActivate(Frame frame)
        {
            var playerLevel = PredictedFrame.Get<PlayerStats>(_entityView.EntityRef);
            var config = PredictedFrame.FindAsset(PredictedFrame.RuntimeConfig.PlayerConfig);
            
            _entityPrototype.PhysicsCollider.IsEnabled = true;
            _entityPrototype.PhysicsCollider.IsTrigger = true;
            _entityPrototype.PhysicsCollider.Shape3D.ShapeType = Shape3DType.Sphere;
            _entityPrototype.PhysicsCollider.Shape3D.SphereRadius = config.AttackRadius.Value(playerLevel.AttackRadiusLevel);

            var playerLink = PredictedFrame.Get<PlayerLink>(_entityView.EntityRef);
            var playerData = PredictedFrame.GetPlayerData(playerLink.Player);
            _playerName.text = playerData.PlayerNickname;

            MessageBroker.Default.Receive<UpgradeButtonClickSignal>().Subscribe(_ =>
            {
                frame.Signals.PlayerToUpgrade(_entityView.EntityRef);
            }).AddTo(_disposable);
        }

        public override void OnDeactivate()
        {
            _disposable.Dispose();
        }
    }
}
