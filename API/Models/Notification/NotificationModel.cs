﻿using DAL.Entities;

namespace API.Models.Notification
{
    public class NotificationModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? SubTitle { get; set; }
        public string? Body { get; set; }
        public Guid ToUserId { get; set; }
        public Guid? FromUserId { get; set; }
        public NotificationType NotificationType { get; set; }

        public DateTimeOffset? ViewedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
