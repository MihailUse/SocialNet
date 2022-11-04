namespace DAL.Entities
{
    public class Follower
    {
        public Guid FollewerId { get; set; }
        public Guid FollowingId { get; set; }

        public User Follewer { get; set; } = null!;
        public User Following { get; set; } = null!;
    }
}
