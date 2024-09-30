using UniRx;
using Signals;

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
            
            f.Signals.EntityDied();
            f.Destroy(owner);
        }
    }
}
