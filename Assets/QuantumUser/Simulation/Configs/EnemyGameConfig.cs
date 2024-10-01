using System;
using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public class EnemyGameConfig : AssetObject
    {
        [Serializable]
        public class EnemyConfig
        {
            [SerializeField] private string _name;
            [SerializeField] private AssetRef<EntityPrototype> _enemyPrototype;
            [SerializeField] private FP _health;

            public string Name => _name;
            public AssetRef<EntityPrototype> EnemyPrototype => _enemyPrototype;
            public FP Health => _health;
        }

        [SerializeField] private List<EnemyConfig> _enemiesConfig = new();

        public IReadOnlyList<EnemyConfig> EnemiesConfig => _enemiesConfig;
    }
}
