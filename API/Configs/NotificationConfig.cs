namespace API.Configs
{
    public class NotificationConfig
    {
        public const string Position = nameof(NotificationConfig);
        public GoogleConfig? Google { get; set; }

        public class GoogleConfig
        {
            public string? ServerKey { get; set; }
            public string? SenderId { get; set; }
            public string? GcmUrl { get; set; }
        }
    }
}
