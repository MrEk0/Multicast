namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>, ISignalOnPlayerAdded
    {
        public override void Update(Frame f, ref Filter filter)
        {
            var input = f.GetPlayerInput(filter.PlayerLink->Player);

            var direction = input->Direction;
            if (direction.Magnitude > 1)
            {
                direction = direction.Normalized;
            }

            filter.KCC->Move(f, filter.Entity, direction.XOY);
        }

        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform3D;
            public CharacterController3D* KCC;
            public PlayerLink* PlayerLink;
        }

        public void OnPlayerAdded(Frame f, PlayerRef player, bool firstTime)
        {
            var runTimePlayer = f.GetPlayerData(player);
            var entity = f.Create(runTimePlayer.PlayerAvatar);

            var link = new PlayerLink()
            {
                Player = player
            };
            f.Add(entity, link);

            if (f.Unsafe.TryGetPointer<Transform3D>(entity, out var transform))
            {
                transform->Position = new FPVector3(player * 2, 2);
            }
        }
    }
}
