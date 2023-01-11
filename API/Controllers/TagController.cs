using API.Extentions;
using API.Models.Tag;
using API.Services;
using Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = SwaggerDefinitionNames.Api)]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;
        private readonly ProjectionGeneratorService _projectionGeneratorService;

        public TagController(TagService tagService, ProjectionGeneratorService projectionGeneratorService)
        {
            _tagService = tagService;
            _projectionGeneratorService = projectionGeneratorService;
        }

        [HttpGet]
        public IEnumerable<TagModel> SearchTags(string? search, int skip = 0, int take = 20)
        {
            _projectionGeneratorService.RequestUserId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return _tagService.SearchTags(search ?? string.Empty, skip, take);
        }

        [HttpGet]
        public Task<TagModel> GetTagById(Guid tagId)
        {
            _projectionGeneratorService.RequestUserId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return _tagService.GetTagById(tagId);
        }

        [HttpPatch]
        public async Task<bool> ChangeFollowStatus(Guid tagId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return await _tagService.ChangeFollowStatus(userId, tagId);
        }
    }
}
