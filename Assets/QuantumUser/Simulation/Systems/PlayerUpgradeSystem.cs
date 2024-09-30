using Signals;
using UniRx;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerUpgradeSystem : SystemSignalsOnly, ISignalEntityDied
    {
        public override void OnInit(Frame f)
        {
            //MessageBroker.Default.Receive<UpgradeButtonClickSignal>().Subscribe(OnUpgradeButtonClicked).AddTo(_disposable);
        }

        public void EntityDied(Frame f)
        {
            MessageBroker.Default.Publish(new EnemyDeathSignal(0));
        }

        private void OnUpgradeButtonClicked(UpgradeButtonClickSignal signal)
        {
            //var level = f.Get<EntityLevel>(EntityRef).Level;
        }
    }
}
