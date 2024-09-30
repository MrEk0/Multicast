using Photon.Deterministic;
using Quantum;
using Signals;
using UnityEngine;
using UnityEngine.UI;

public class EntityHpView : QuantumEntityViewComponent
{
    [SerializeField] private Image _hpBarImage;

    public override void OnInitialize()
    {
        _hpBarImage.type = Image.Type.Filled;
        _hpBarImage.fillMethod = Image.FillMethod.Horizontal;
        _hpBarImage.fillAmount = 1f;
    }

    public override void OnUpdateView()
    {
        var health = PredictedFrame.Get<EntityHealth>(EntityRef);
        if (health.MaxHealthPoints == 0)
            return;
        
        _hpBarImage.fillAmount = health.HealthPoints.AsFloat / health.MaxHealthPoints.AsFloat;
    }
}