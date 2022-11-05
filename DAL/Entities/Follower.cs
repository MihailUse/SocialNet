namespace DAL.Entities
{
    public class Follower
    {
        public Guid FollewerId { get; set; }
        public Guid FollowingId { get; set; }

        public virtual User Follewer { get; set; } = null!;
        public virtual User Following { get; set; } = null!;

        public Follower(Guid follewerId, Guid followingId)
        {
            FollewerId = follewerId;
            FollowingId = followingId;
        }
    }
}
