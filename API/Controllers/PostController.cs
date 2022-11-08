using API.Models.Auth;
using API.Models.Post;
using API.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        public IEnumerable<PostModel> GetPosts()
        {
            return _postService.GetPosts()
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

        [HttpGet]
        public async Task<PostModel> GetPostById(Guid id)
        {
            Post post = await _postService.GetPostById(id);
            return _mapper.Map<PostModel>(post);
        }

        [HttpPost]
        public async Task<Guid> CreatePost(CreatePostModel model)
        {
            Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == TokenClaimTypes.UserId)?.Value, out Guid id);
            Post post = _mapper.Map<Post>(model);

            return await _postService.CreatePost(id, post);
        }
    }
}
