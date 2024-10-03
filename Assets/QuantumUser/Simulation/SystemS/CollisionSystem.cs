namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerAttackSystem : SystemSignalsOnly, ISignalOnTrigger3D, ISignalOnTriggerEnter3D, ISignalOnTriggerExit3D
    {
        public void OnTrigger3D(Frame f, TriggerInfo3D info)
        {
            if (f.Has<PlayerLink>(info.Entity))
                f.Events.OnPlayerEntityTriggerStay(info.Other, info.Entity);
        }

        public void OnTriggerEnter3D(Frame f, TriggerInfo3D info)
        {
            if (f.Has<PlayerLink>(info.Entity))
                f.Events.OnPlayerEntityTriggerEnter(info.Other);
        }

        public void OnTriggerExit3D(Frame f, ExitInfo3D info)
        {
            if (f.Has<PlayerLink>(info.Entity))
                f.Events.OnPlayerEntityTriggerExit(info.Other);
        }
    }
}
