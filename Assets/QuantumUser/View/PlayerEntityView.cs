using Quantum;
using TMPro;
using UnityEngine;

public class PlayerEntityView : QuantumViewComponent<MapViewContext>
{
    [SerializeField] private QuantumEntityPrototype _attackCollider;
    [SerializeField] private TMP_Text _playerName;

    public override void OnActivate(Frame frame)
    {
        var tr = transform;
        ViewContext.VirtualCameraBase.Follow = tr;
        ViewContext.VirtualCameraBase.LookAt = tr;
        
        var level = PredictedFrame.Get<EntityLevel>(_entityView.EntityRef).Level;
        var config = PredictedFrame.FindAsset(PredictedFrame.RuntimeConfig.PlayerConfig);

        _attackCollider.PhysicsCollider.IsEnabled = true;
        _attackCollider.PhysicsCollider.IsTrigger = true;
        _attackCollider.PhysicsCollider.Shape3D.ShapeType = Shape3DType.Sphere;
        _attackCollider.PhysicsCollider.Shape3D.SphereRadius = config.AttackRadius.Value(level);
        
        var playerLink = PredictedFrame.Get<PlayerLink>(_entityView.EntityRef);
        var playerData = PredictedFrame.GetPlayerData(playerLink.Player);
        _playerName.text = playerData.PlayerNickname;
    }
}
