using API.Models.Post;
using API.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Constants;
using Common.Extentions;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly PostService _postService;

        public PostController(IMapper mapper, PostService postService)
        {
            _mapper = mapper;
            _postService = postService;
        }

        [HttpGet]
        public IEnumerable<PostModel> GetPosts(int skip = 0, int take = 20)
        {
            return _postService.GetPosts(skip, take)
                .ProjectTo<PostModel>(_mapper.ConfigurationProvider)
                .AsEnumerable();
        }

        [HttpGet]
        public IEnumerable<PostModel> GetPostsByAuthor(Guid userId)
        {
            return _postService.GetPostsByAuthor(userId)
                .ProjectTo<PostModel>(_mapper.ConfigurationProvider)
                .AsEnumerable();
        }

        [HttpPost]
        public async Task<Guid> CreatePost(CreatePostModel model)
        {
            Post post = _mapper.Map<Post>(model);
            post.AuthorId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);

            return await _postService.CreatePost(post);
        }

        [HttpDelete]
        public async Task DeletePost(Guid postId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _postService.DeletePost(userId, postId);
        }
    }
}
