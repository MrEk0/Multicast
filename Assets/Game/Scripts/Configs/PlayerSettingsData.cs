using System;
using UnityEngine;

namespace Game.Scripts.Configs
{
    [CreateAssetMenu(menuName = "Configs/PlayerSettingsData")]
    public class PlayerSettingsData : ScriptableObject
    {
        [Serializable]
        public class SettingParameters
        {
            [SerializeField] private float _initialValue;
            [SerializeField] private float _upgradeStep;
            [SerializeField] private float _upgradeChance;

            public float InitialValue => _initialValue;
            public float UpgradeStep => _upgradeStep;
            public float UpgradeChance => _upgradeChance;
        }

        [SerializeField] private SettingParameters _velocity;
        [SerializeField] private SettingParameters _damage;
        [SerializeField] private SettingParameters _attackRadius;

        public SettingParameters Velocity => _velocity;
        public SettingParameters Damage => _damage;
        public SettingParameters AttackRadius => _attackRadius;
    }
}