using API.Models.Attach;

namespace API.Services
{
    public class AttachService
    {
        private readonly string _tempPath;
        private readonly string _attachPath;

        public AttachService()
        {
            _tempPath = Path.GetTempPath();
            _attachPath = Path.Combine(Directory.GetCurrentDirectory(), "Attaches");
            Directory.CreateDirectory(_tempPath);
            Directory.CreateDirectory(_attachPath);
        }

        public async Task<MetadataModel> SaveTempFile(IFormFile file)
        {
            MetadataModel metadata = new MetadataModel(file.FileName, file.ContentType, file.Length);
            string filePath = Path.Combine(_tempPath, metadata.Id.ToString());

            using (var stream = File.Create(filePath))
                await file.CopyToAsync(stream);

            return metadata;
        }

        public void SaveAttach(Guid fileName)
        {
            string filePath = Path.Combine(_attachPath, fileName.ToString());
            string tempFilePath = Path.Combine(_tempPath, fileName.ToString());

            File.Copy(tempFilePath, filePath, true);
        }

        public FileStream GetStream(Guid id)
        {
            string filePath = Path.Combine(_attachPath, id.ToString());
            return new FileStream(filePath, FileMode.Open);
        }
    }
}
