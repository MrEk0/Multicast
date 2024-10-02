using UniRx;
using UnityEngine;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerStatsSystem : SystemSignalsOnly, ISignalPlayerToUpgrade, ISignalEntityDied
    {
        public override void OnInit(Frame f)
        {
            MessageBroker.Default.Publish(new EnemyKillsChange(0));
        }

        public void PlayerToUpgrade(Frame f, EntityRef player)
        {
            var config = f.FindAsset(f.RuntimeConfig.PlayerConfig);

            var playerStats = f.Get<PlayerStats>(player);

            var rndChance = Random.Range(0f, 1f);

            playerStats.DamageLevel = rndChance > config.AttackRadius.UpgradeChance && rndChance <= config.Velocity.UpgradeChance ? playerStats.DamageLevel + 1 : playerStats.DamageLevel;
            playerStats.AttackRadiusLevel = rndChance <= config.AttackRadius.UpgradeChance ? playerStats.AttackRadiusLevel + 1 : playerStats.AttackRadiusLevel;
            playerStats.VelocityLevel = rndChance > config.Velocity.UpgradeChance ? playerStats.VelocityLevel + 1 : playerStats.VelocityLevel;
            
            f.Set(player, playerStats);
            
            f.Signals.PlayerVelocityUpgrade(config.Velocity.Value(playerStats.VelocityLevel));

            MessageBroker.Default.Publish(new PlayerLevelUpSignal(config.Damage.Value(playerStats.DamageLevel).AsFloat, config.AttackRadius.Value(playerStats.AttackRadiusLevel).AsFloat, config.Velocity.Value(playerStats.VelocityLevel).AsFloat));
        }

        public void EntityDied(Frame f, EntityRef deadEntity, EntityRef killer)
        {
            var playerStats = f.Get<PlayerStats>(killer);
            playerStats.KillCount += 1;
            f.Set(killer, playerStats);

            MessageBroker.Default.Publish(new EnemyKillsChange(playerStats.KillCount.AsInt));
        }
    }
}
