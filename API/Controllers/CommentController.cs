﻿using API.Extentions;
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
        private readonly ProjectionGeneratorService _projectionGeneratorService;

        public CommentController(CommentService commentService, ProjectionGeneratorService projectionGeneratorService)
        {
            _commentService = commentService;
            _projectionGeneratorService = projectionGeneratorService;

            _projectionGeneratorService.AvatarLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new { userId = x.Id });
        }

        [HttpGet]
        public IEnumerable<CommentModel> GetPostComments(Guid postId, int skip = 0, int take = 20)
        {
            _projectionGeneratorService.RequestUserId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return _commentService.GetPostComments(postId, skip, take);
        }

        [HttpPost]
        public async Task<Guid> CreateComment(CreateCommentModel createModel)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return await _commentService.CreateComment(userId, createModel);
        }

        [HttpPatch]
        public async Task<bool> ChangeLikeStatus(Guid commentId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return await _commentService.ChangeLikeStatus(userId, commentId);
        }

        [HttpDelete]
        public async Task DeleteComment(Guid commentId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _commentService.DeleteComment(userId, commentId);
        }
    }
}
