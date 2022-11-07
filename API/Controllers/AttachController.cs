using API.Models.Attach;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {
        private readonly AttachService _attachService;

        public AttachController(AttachService attachService)
        {
            _attachService = attachService;
        }

        [HttpPost]
        public async Task<List<MetadataModel>> UploadMultipleFiles(IEnumerable<IFormFile> files)
        {
            List<MetadataModel> metadatas = new List<MetadataModel>();

            foreach (var file in files)
                metadatas.Add(await _attachService.SaveTempFile(file));

            return metadatas;
        }

        [HttpPost]
        public async Task<MetadataModel> UploadFile(IFormFile file)
        {
            return await _attachService.SaveTempFile(file);
        }

        [HttpPost]
        public FileStreamResult GetFile(MetadataModel metadata)
        {
            FileStream fs = _attachService.GetStream(metadata.Id);
            return File(fs, metadata.MimeType);
        }

        [HttpPost]
        public FileResult DownloadFile(MetadataModel metadata)
        {
            FileStream fs = _attachService.GetStream(metadata.Id);
            return File(fs, metadata.MimeType, metadata.Name);
        }
    }
}
