namespace API.Models.Notification
{
    public class SendNotificationModel
    {
        public Guid? UserId { get; set; }
        public NotificationModel Notification { get; set; } = null!;
    }
}
