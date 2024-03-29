﻿namespace API.Models.Attach
{
    public class MetadataModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long Size { get; set; } // in bytes
    }
}
