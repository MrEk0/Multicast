using UnityEngine.Scripting;

namespace Quantum
{
    [Preserve]
    public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerAdded
    {
        public void OnPlayerAdded(Frame f, PlayerRef player, bool firstTime)
        {
            var gameConfig = f.FindAsset(f.RuntimeConfig.GameConfig);

            var runTimePlayer = f.GetPlayerData(player);
            var entity = f.Create(runTimePlayer.PlayerAvatar);

            f.Add(entity, new PlayerLink { Player = player });

            if (f.Unsafe.TryGetPointer<Transform3D>(entity, out var transform))
            {
                transform->Position = gameConfig.PlayerSpawnPoint;
            }
        }
    }
}
