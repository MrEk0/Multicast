using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quantum
{
    public class EntityHpView : QuantumEntityViewComponent
    {
        [SerializeField] private Image _hpBarImage;
        [SerializeField] private TMP_Text _name;

        public override void OnActivate(Frame frame)
        {
            _hpBarImage.type = Image.Type.Filled;
            _hpBarImage.fillMethod = Image.FillMethod.Horizontal;
            _hpBarImage.fillAmount = 1f;
            
            var entityName = frame.Get<EntityName>(_entityView.EntityRef);
            var config = frame.FindAsset(frame.RuntimeConfig.EnemyConfig);
            if (config.EnemiesConfig.Count == 0 || config.EnemiesConfig.Count <= entityName.nameIndex.AsInt)
                return;
            
            _name.text = config.EnemiesConfig[entityName.nameIndex.AsInt].Name;
        }

        public override void OnUpdateView()
        {
            var health = PredictedFrame.Get<EntityHealth>(EntityRef);
            if (health.MaxHealthPoints == 0)
                return;

            _hpBarImage.fillAmount = health.HealthPoints.AsFloat / health.MaxHealthPoints.AsFloat;
        }
    }
}