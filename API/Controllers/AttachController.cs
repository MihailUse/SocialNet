using API.Models.Attach;
using API.Services;
using Common.Constants;
using Common.Extentions;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = SwaggerDefinitionNames.Api)]
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

        [HttpGet]
        public async Task<FileResult> GetUserAvatar(Guid userId, bool download = false)
        {
            Avatar avatar = await _userService.GetUserAvatar(userId);
            return RenderAttach(avatar, download);
        }

        [HttpGet]
        [Authorize]
        public async Task<FileResult> GetUserAttach(Guid attachId, bool download = false)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            Attach attach = await _userService.GetUserAttach(userId, attachId);
            return RenderAttach(attach, download);
        }

        [HttpGet]
        public async Task<FileResult> GetPostAttach(Guid postId, Guid attachId, bool download = false)
        {
            Attach attach = await _postService.GetPostAttach(postId, attachId);
            return RenderAttach(attach, download);
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

        private FileStreamResult RenderAttach(Attach attach, bool download)
        {
            FileStream fs = _attachService.GetStream(attach.Id);

            if (download)
                return File(fs, attach.MimeType, attach.Name);

            return File(fs, attach.MimeType);
        }
    }
}
