using TMPro;
using UniRx;
using UnityEngine;

namespace Quantum
{
    public class UISceneView : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button _upgradeButton;
        [SerializeField] private TMP_Text _velocityText;
        [SerializeField] private TMP_Text _attackRadiusText;
        [SerializeField] private TMP_Text _damageText;
        [SerializeField] private TMP_Text _killsText;

        private readonly CompositeDisposable _disposable = new();

        private void OnEnable()
        {
            MessageBroker.Default.Receive<PlayerLevelUpSignal>().Subscribe(OnPlayerLevelUp).AddTo(_disposable);
            MessageBroker.Default.Receive<EnemyKillsChange>().Subscribe(OnEnemyDeath).AddTo(_disposable);

            _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }

        private void OnDisable()
        {
            _disposable.Dispose();

            _upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
        }

        private void OnUpgradeButtonClicked()
        {
            MessageBroker.Default.Publish(new UpgradeButtonClickSignal());
        }

        private void OnPlayerLevelUp(PlayerLevelUpSignal signal)
        {
            _velocityText.text = string.Format($"{signal.Velocity:F1}");
            _attackRadiusText.text = string.Format($"{signal.AttackRadius:F0}");
            _damageText.text = string.Format($"{signal.Damage:F0}");
        }

        private void OnEnemyDeath(EnemyKillsChange signal)
        {
            _killsText.text = signal.DeathCount.ToString();
        }
    }
}