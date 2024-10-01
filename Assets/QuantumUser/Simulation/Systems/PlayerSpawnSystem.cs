using Photon.Deterministic;
using UniRx;
using UnityEngine.Scripting;

namespace Quantum
{
    [Preserve]
    public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerAdded
    {
        public void OnPlayerAdded(Frame f, PlayerRef player, bool firstTime)
        {
            var config = f.FindAsset(f.RuntimeConfig.PlayerConfig);
            var runTimePlayer = f.GetPlayerData(player);
            var entity = f.Create(runTimePlayer.PlayerAvatar);
            
            f.Add(entity, new PlayerLink { Player = player });

            if (f.Unsafe.TryGetPointer<Transform3D>(entity, out var transform))
            {
                transform->Position = new FPVector3(0, 2, 0);//todo redo
            }
            
            MessageBroker.Default.Publish(new PlayerLevelUpSignal(config.Damage.Value(0).AsFloat,
                config.AttackRadius.Value(0).AsFloat, config.Velocity.Value(0).AsFloat));
        }
    }
}
