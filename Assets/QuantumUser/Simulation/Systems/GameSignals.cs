namespace Signals
{
    public struct UpgradeButtonClickSignal
    {
        
    }

    public struct EnemyDeathSignal
    {
        public int DeathCount { get; private set; }

        public EnemyDeathSignal(int currentDeath)
        {
            DeathCount = currentDeath;
        }
    }

    public struct PlayerLevelUpSignal
    {
        public float Damage { get; private set; }
        public float AttackRadius { get; private set; }
        public float Velocity { get; private set; }
        
        public PlayerLevelUpSignal(float currentDamage, float currentAttackRadius, float currentVelocity)
        {
            Damage = currentDamage;
            AttackRadius = currentAttackRadius;
            Velocity = currentVelocity;
        }
    }
}