using API.Models.Attach;
using API.Models.Auth;
using API.Models.User;
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
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly AttachService _attachService;

        public UserController(IMapper mapper, UserService userService, AttachService attachService)
        {
            _mapper = mapper;
            _userService = userService;
            _attachService = attachService;
        }

        [HttpGet]
        public IEnumerable<UserModel> GetUsers()
        {
            return _userService.GetUsers()
                .ProjectTo<UserModel>(_mapper.ConfigurationProvider)
                .AsEnumerable();
        }

        [HttpGet]
        public async Task<UserModel> GetUserById(Guid id)
        {
            User user = await _userService.GetUserById(id);
            return _mapper.Map<UserModel>(user);
        }

        [HttpGet]
        public async Task<FileResult> GetUserAvatar(Guid id)
        {
            Avatar avatar = await _userService.GetUserAvatar(id);
            FileStream fs = _attachService.GetStream(avatar.Id);
            return File(fs, avatar.MimeType);
        }

        [HttpGet]
        [Authorize]
        public IQueryable<Attach> GetUserAttaches()
        {
            Guid.TryParse(User.FindFirst(TokenClaimTypes.UserId)?.Value, out Guid id);
            return _userService.GetUserAttaches(id);
        }

        [HttpPost]
        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            User user = _mapper.Map<User>(model);
            return await _userService.CreateUser(user);
        }

        [HttpPost]
        [Authorize]
        public async Task SetUserAvatar(MetadataModel metadata)
        {
            Guid.TryParse(User.FindFirst(TokenClaimTypes.UserId)?.Value, out Guid id);
            await _userService.SetUserAvatar(id, metadata);
        }

        [HttpPost]
        [Authorize]
        public async Task ChangeFollowStatus(Guid followingId)
        {
            Guid.TryParse(User.FindFirst(TokenClaimTypes.UserId)?.Value, out Guid followerId);
            await _userService.ChangeFollowStatus(followerId, followingId);
        }

        [HttpPatch]
        [Authorize]
        public async Task UpdateUser(UpdateUserModel model)
        {
            Guid.TryParse(User.FindFirst(TokenClaimTypes.UserId)?.Value, out Guid id);
            User user = _mapper.Map<User>(model);
            await _userService.UpdateUser(id, user);
        }

        [HttpDelete]
        [Authorize]
        public async Task DeleteUser()
        {
            Guid.TryParse(User.FindFirst(TokenClaimTypes.UserId)?.Value, out Guid id);
            await _userService.DeleteUser(id);
        }
    }
}
