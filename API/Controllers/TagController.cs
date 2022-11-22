using API.Extentions;
using API.Models.Tag;
using API.Services;
using Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = SwaggerDefinitionNames.Api)]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public IEnumerable<SearchTagModel> SearchTags([MinLength(2)] string search, int take = 10)
        {
            return _tagService.SearchTags(search, take);
        }

        [HttpGet]
        public Task<TagInfoModel> GetTagInfoModel(Guid tagId)
        {
            return _tagService.GetTagInfoModel(tagId);
        }

        [HttpPost]
        public async Task ChangeFollowStatus(Guid tagId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _tagService.ChangeFollowStatus(userId, tagId);
        }
    }
}
