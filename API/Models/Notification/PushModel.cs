namespace API.Models.Notification
{
    public class PushModel
    {
        public int? Badge { get; set; }
        public string? Sound { get; set; }
        public AlertModel Alert { get; set; } = null!;


        public Dictionary<string, string?>? CustomData { get; set; }
    }
}
