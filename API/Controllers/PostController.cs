using API.Extentions;
using API.Models.Post;
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
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService, ProjectionGeneratorService projectionGeneratorService)
        {
            _postService = postService;

            projectionGeneratorService.PostAttachLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetPostAttach), new { postId = x.PostId, attachId = x.Id });
            projectionGeneratorService.AvatarLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new { userId = x.UserId });
        }

        // for testing
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<PostModel> GetPosts(int skip = 0, int take = 20, Guid? requestUserId = null)
        {
            return _postService.GetPosts(skip, take, requestUserId ?? Guid.Empty);
        }

        [HttpGet]
        public IEnumerable<PostModel> GetPersonalPosts(int skip = 0, int take = 20)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return _postService.GetPersonalPosts(userId, skip, take, userId);
        }

        [HttpGet]
        public IEnumerable<PostModel> GetUserPosts(Guid userId, int skip = 0, int take = 20)
        {
            Guid requestUserId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return _postService.GetUserPosts(userId, skip, take, requestUserId);
        }

        [HttpPost]
        public async Task<Guid> CreatePost(CreatePostModel createModel)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return await _postService.CreatePost(userId, createModel);
        }

        [HttpPost]
        public async Task<bool> ChangeLikeStatus(Guid postId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return await _postService.ChangeLikeStatus(userId, postId);
        }

        [HttpDelete]
        public async Task DeletePost(Guid postId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _postService.DeletePost(userId, postId);
        }
    }
}
