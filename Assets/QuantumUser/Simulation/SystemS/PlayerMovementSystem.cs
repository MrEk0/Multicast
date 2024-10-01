using Photon.Deterministic;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerMovementSystem : SystemMainThreadFilter<PlayerMovementSystem.Filter>, ISignalPlayerVelocityUpgrade
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform3D;
            public CharacterController3D* Controller3D;
            public PlayerLink* PlayerLink;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            var input = f.GetPlayerInput(filter.PlayerLink->Player);

            var direction = input->Direction.Normalized;

            filter.Controller3D->Move(f, filter.Entity, direction.XOY);
        }

        public void PlayerVelocityUpgrade(Frame f, FP velocity)
        {
            var filter = f.Filter<CharacterController3D>();
            while (filter.Next(out var entity, out var character))
            {
                character.MaxSpeed = velocity;
                f.Set(entity, character);
            }
        }
    }
}
