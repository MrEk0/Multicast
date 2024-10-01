using TMPro;
using UnityEngine;

namespace Quantum
{
    public class PlayerEntityView : QuantumViewComponent<MapViewContext>
    {
        [SerializeField] private TMP_Text _playerName;

        public override void OnActivate(Frame frame)
        {
            var tr = transform;
            ViewContext.VirtualCameraBase.Follow = tr;
            ViewContext.VirtualCameraBase.LookAt = tr;

            var playerLink = PredictedFrame.Get<PlayerLink>(_entityView.EntityRef);
            var playerData = PredictedFrame.GetPlayerData(playerLink.Player);
            _playerName.text = playerData.PlayerNickname;
        }
    }
}
