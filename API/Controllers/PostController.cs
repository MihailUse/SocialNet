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
        private readonly ProjectionGeneratorService _projectionGeneratorService;

        public PostController(PostService postService, ProjectionGeneratorService projectionGeneratorService)
        {
            _postService = postService;
            _projectionGeneratorService = projectionGeneratorService;

            _projectionGeneratorService.PostAttachLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetPostAttach), new { postId = x.PostId, attachId = x.Id });
            _projectionGeneratorService.AvatarLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new { userId = x.UserId });
        }

        // for testing
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<PostModel> GetPosts(int skip = 0, int take = 20, Guid? requestUserId = null)
        {
            _projectionGeneratorService.RequestUserId = requestUserId ?? Guid.Empty;
            return _postService.GetPosts(skip, take);
        }

        [HttpGet]
        public IEnumerable<PostModel> GetPersonalPosts(int skip = 0, int take = 20)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            _projectionGeneratorService.RequestUserId = userId;
            return _postService.GetPersonalPosts(userId, skip, take);
        }

        [HttpGet]
        public IEnumerable<PostModel> GetUserPosts(Guid userId, int skip = 0, int take = 20)
        {
            _projectionGeneratorService.RequestUserId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return _postService.GetUserPosts(userId, skip, take);
        }

        [HttpGet]
        public IEnumerable<PostModel> GetPostsByTag(Guid tagId, int skip = 0, int take = 20)
        {
            _projectionGeneratorService.RequestUserId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return _postService.GetPostsByTag(tagId, skip, take);
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
