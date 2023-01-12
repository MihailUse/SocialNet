namespace API.Models.Notification
{
    public class AlertModel
    {
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Body { get; set; }

        public AlertModel(string title, string? subtitle, string? body)
        {
            Title = title;
            Subtitle = subtitle;
            Body = body;
        }
    }
}
