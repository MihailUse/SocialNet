﻿using API.Exceptions;
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
            MetadataModel metadata = new MetadataModel()
            {
                Name = file.FileName,
                MimeType = file.ContentType,
                Size = file.Length
            };
            string filePath = Path.Combine(_tempPath, metadata.Id.ToString());

            using (var stream = File.Create(filePath))
                await file.CopyToAsync(stream);

            return metadata;
        }

        public void SaveAttach(Guid fileId)
        {
            string filePath = Path.Combine(_attachPath, fileId.ToString());
            string tempFilePath = Path.Combine(_tempPath, fileId.ToString());

            if (!File.Exists(tempFilePath))
                throw new NotFoundServiceException("Temp file not found");

            File.Move(tempFilePath, filePath, true);
        }

        public FileStream GetStream(Guid fileId)
        {
            string filePath = Path.Combine(_attachPath, fileId.ToString());

            if (!File.Exists(filePath))
                throw new NotFoundServiceException("File file not found");

            return new FileStream(filePath, FileMode.Open);
        }
    }
}
