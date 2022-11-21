﻿namespace DAL.Entities
{
    public class PostLike
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }

        public User User { get; set; } = null!;
        public Post Post { get; set; } = null!;

        public PostLike(Guid userId, Guid postId)
        {
            UserId = userId;
            PostId = postId;
        }
    }
}
