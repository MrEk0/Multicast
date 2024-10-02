using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum
{
    [Preserve]
    public unsafe class EnemySpawnSystem : SystemSignalsOnly, ISignalEntityDied
    {
        public override void OnInit(Frame f)
        {
            var gameConfig = f.FindAsset(f.RuntimeConfig.GameConfig);

            for (var i = 0; i < gameConfig.MaxEnemiesCount; i++)
                InitEnemy(f, gameConfig);
        }

        private void InitEnemy(Frame f, GameConfig gameConfig)
        {
            var enemyConfig = f.FindAsset(f.RuntimeConfig.EnemyConfig);
            
            var index = f.RNG->Next(0, enemyConfig.EnemiesConfig.Count);
            var config = enemyConfig.EnemiesConfig[index];
            var prototype = config.EnemyPrototype;
                
            SpawnEnemy(f, gameConfig, index, config.Health, prototype);
        }

        private void SpawnEnemy(Frame f, GameConfig config, FP index, FP healthPoints, AssetRef<EntityPrototype> prototype)
        {
            var spawnPosition = new FPVector2(f.RNG->Next(-config.MapExtends.X, config.MapExtends.X),
                f.RNG->Next(-config.MapExtends.Y, config.MapExtends.Y));
            
            var enemy = f.Create(prototype);
            var asteroidTransform = f.Unsafe.GetPointer<Transform3D>(enemy);

            if (IsOutOfBounds(spawnPosition, config.MapExtends, out var newPosition))
                asteroidTransform->Position = newPosition.XOY;
            else
                asteroidTransform->Position = spawnPosition.XOY;
            
            var health = f.Get<EntityHealth>(enemy);
            health.HealthPoints = healthPoints;
            health.MaxHealthPoints = healthPoints;
            f.Set(enemy, health);

            var name = f.Get<EntityName>(enemy);
            name.nameIndex = index;
            f.Set(enemy, name);
        }

        private bool IsOutOfBounds(FPVector2 position, FPVector2 mapExtends, out FPVector2 newPosition)
        {
            newPosition = position;

            if (position.X >= -mapExtends.X && position.X <= mapExtends.X && position.Y >= -mapExtends.Y && position.Y <= mapExtends.Y)
                return false;
            
            var x = FPMath.Clamp(position.X, -mapExtends.X, mapExtends.X);
            var y = FPMath.Clamp(position.Y, -mapExtends.Y, mapExtends.Y);

            newPosition = new FPVector2(x, y);

            return true;
        }

        public void EntityDied(Frame f, EntityRef entity)
        {
            var gameConfig = f.FindAsset(f.RuntimeConfig.GameConfig);

            InitEnemy(f, gameConfig);
        }
    }
}