using UniRx;

namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class HealthSystem : SystemSignalsOnly, ISignalEntityHit
    {
        public void EntityHit(Frame f, EntityRef owner, FP damage)
        {
            if (!f.Unsafe.TryGetPointer<EntityHealth>(owner, out var health))
                return;

            health->HealthPoints -= damage;

            if (health->HealthPoints > 0)
                return;

            MessageBroker.Default.Publish(new EntityDeathSignal());
            f.Destroy(owner);
        }
    }
}
