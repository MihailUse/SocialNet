namespace API.Models.Notification
{
    public class SendNotificationModel
    {
        public Guid? UserId { get; set; }
        public PushModel Notification { get; set; } = null!;
    }
}
