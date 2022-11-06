namespace API.Models.Attach
{
    public class MetadataModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long Size { get; set; } // in bytes

        public MetadataModel(string name, string mimeType, long size)
        {
            Name = name;
            MimeType = mimeType;
            Size = size;
        }
    }
}
