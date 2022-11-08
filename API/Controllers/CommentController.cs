using API.Constants;
using API.Models.Comment;
using API.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Extentions;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CommentService _commentService;

        public CommentController(IMapper mapper, CommentService commentService)
        {
            _mapper = mapper;
            _commentService = commentService;
        }

        [HttpGet]
        public IEnumerable<CommentModel> GetPostComments(Guid postId)
        {
            return _commentService.GetCommentsByPost(postId)
                .ProjectTo<CommentModel>(_mapper.ConfigurationProvider)
                .AsEnumerable();
        }

        [HttpPost]
        public async Task<Guid> CreateComment(CreateCommentModel model)
        {
            Comment comment = _mapper.Map<Comment>(model);
            comment.AuthorId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);

            return await _commentService.CreateComment(comment);
        }

        [HttpDelete]
        public async Task DeleteComment(Guid commentId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _commentService.DeleteComment(userId, commentId);
        }
    }
}
