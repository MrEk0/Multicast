using TMPro;
using UnityEngine;

namespace Quantum
{
    public class PlayerEntityView : QuantumViewComponent<MapViewContext>
    {
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private QuantumEntityPrototype _entityPrototype;
        
        public override void OnActivate(Frame frame)
        {
            var tr = transform;
            ViewContext.VirtualCameraBase.Follow = tr;
            ViewContext.VirtualCameraBase.LookAt = tr;

            var playerLevel = PredictedFrame.Get<PlayerEntityLevel>(_entityView.EntityRef);
            var config = PredictedFrame.FindAsset(PredictedFrame.RuntimeConfig.PlayerConfig);
            
            _entityPrototype.PhysicsCollider.IsEnabled = true;
            _entityPrototype.PhysicsCollider.IsTrigger = true;
            _entityPrototype.PhysicsCollider.Shape3D.ShapeType = Shape3DType.Sphere;
            _entityPrototype.PhysicsCollider.Shape3D.SphereRadius = config.AttackRadius.Value(playerLevel.AttackRadiusLevel);

            var playerLink = PredictedFrame.Get<PlayerLink>(_entityView.EntityRef);
            var playerData = PredictedFrame.GetPlayerData(playerLink.Player);
            _playerName.text = playerData.PlayerNickname;
        }
    }
}
