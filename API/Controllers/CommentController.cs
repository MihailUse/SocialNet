using API.Extentions;
using API.Models.Comment;
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
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService, LinkGeneratorService linkGenerator)
        {
            _commentService = commentService;

            linkGenerator.AvatarLinkGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new { userId = x.UserId });
        }

        [HttpGet]
        public IEnumerable<CommentModel> GetPostComments(Guid postId, int skip = 0, int take = 20)
        {
            return _commentService.GetPostComments(postId, skip, take);
        }

        [HttpPost]
        public async Task<Guid> CreateComment(CreateCommentModel createModel)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return await _commentService.CreateComment(userId, createModel);
        }

        [HttpPost]
        public async Task ChangeLikeStatus(Guid commentId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _commentService.ChangeLikeStatus(userId, commentId);
        }

        [HttpDelete]
        public async Task DeleteComment(Guid commentId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _commentService.DeleteComment(userId, commentId);
        }
    }
}
