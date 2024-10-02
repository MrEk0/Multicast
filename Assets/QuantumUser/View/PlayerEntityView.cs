using TMPro;
using UniRx;
using UnityEngine;

namespace Quantum
{
    public class PlayerEntityView : QuantumViewComponent<SceneViewContext>
    {
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private QuantumEntityPrototype _entityPrototype;
        [SerializeField] private Vector3 _cameraOffset;

        private bool _isLocal;
        
        private readonly CompositeDisposable _disposable = new();

        public override void OnActivate(Frame frame)
        {
            var playerLevel = frame.Get<PlayerStats>(_entityView.EntityRef);
            var playerLink = frame.Get<PlayerLink>(_entityView.EntityRef);
            var playerData = frame.GetPlayerData(playerLink.Player);
            
            var config = frame.FindAsset(frame.RuntimeConfig.PlayerConfig);
            _isLocal = Game.PlayerIsLocal(playerLink.Player);
            
            _entityPrototype.PhysicsCollider.IsEnabled = true;
            _entityPrototype.PhysicsCollider.IsTrigger = true;
            _entityPrototype.PhysicsCollider.Shape3D.ShapeType = Shape3DType.Sphere;
            _entityPrototype.PhysicsCollider.Shape3D.SphereRadius = config.AttackRadius.Value(playerLevel.AttackRadiusLevel);
            
            _playerName.text = playerData.PlayerNickname;

            MessageBroker.Default.Receive<UpgradeButtonClickSignal>().Subscribe(_ =>
            {
                frame.Signals.PlayerToUpgrade(_entityView.EntityRef);
            }).AddTo(_disposable);
        }

        public override void OnUpdateView()
        {
            if (!_isLocal)
                return;

            var tr = transform;
            ViewContext.CharacterCamera.transform.position = tr.position + _cameraOffset;
            ViewContext.CharacterCamera.transform.LookAt(tr);
        }

        public override void OnDeactivate()
        {
            _disposable.Dispose();
        }
    }
}
