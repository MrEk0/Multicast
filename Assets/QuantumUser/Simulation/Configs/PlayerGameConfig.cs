using System;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public class PlayerGameConfig : AssetObject
    {
        [Serializable]
        public class SettingParameters
        {
            [SerializeField] private FP _initialValue;
            [SerializeField] private FP _upgradeStep;
            [SerializeField] private FP _upgradeChance;

            public FP Value(FP level) => _initialValue + _upgradeStep * level;
            public FP UpgradeChance => _upgradeChance;
        }

        [SerializeField] private SettingParameters _velocity;
        [SerializeField] private SettingParameters _damage;
        [SerializeField] private SettingParameters _attackRadius;

        public SettingParameters Velocity => _velocity;
        public SettingParameters Damage => _damage;
        public SettingParameters AttackRadius => _attackRadius;
    }
}