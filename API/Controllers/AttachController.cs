using API.Models.Attach;
using API.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly AttachService _attachService;

        public AttachController(UserService userService, PostService postService, AttachService attachService)
        {
            _userService = userService;
            _postService = postService;
            _attachService = attachService;
        }

        [HttpPost]
        [Authorize]
        public async Task<List<MetadataModel>> UploadMultipleFiles(IEnumerable<IFormFile> files)
        {
            List<MetadataModel> metadatas = new List<MetadataModel>();

            foreach (var file in files)
                metadatas.Add(await _attachService.SaveTempFile(file));

            return metadatas;
        }

        [HttpPost]
        [Authorize]
        public async Task<MetadataModel> UploadFile(IFormFile file)
        {
            return await _attachService.SaveTempFile(file);
        }

        [HttpGet]
        public async Task<FileResult> GetUserAvatar(Guid attachId, bool download = false)
        {
            Attach attach = await _userService.GetUserAvatar(attachId);
            return RenderAttach(attach, download);
        }

        [HttpGet]
        public async Task<FileResult> GetPostAttach(Guid attachId, bool download = false)
        {
            Attach attach = await _postService.GetPostAttach(attachId);
            return RenderAttach(attach, download);
        }

        private FileStreamResult RenderAttach(Attach attach, bool download)
        {
            FileStream fs = _attachService.GetStream(attach.Id);

            if (download)
                return File(fs, attach.MimeType, attach.Name);

            return File(fs, attach.MimeType);
        }
    }
}
