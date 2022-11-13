using API.Models.Attach;
using API.Models.User;
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
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public UserController(IMapper mapper, UserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet]
        public IEnumerable<UserMiniModel> GetUsers()
        {
            return _userService.GetUsers()
                .ProjectTo<UserMiniModel>(_mapper.ConfigurationProvider)
                .AsEnumerable();
        }

        [HttpGet]
        public async Task<UserMiniModel> GetUserById(Guid userId)
        {
            User user = await _userService.GetUserById(userId);
            return _mapper.Map<UserMiniModel>(user);
        }

        [HttpGet]
        public UserModel? GetUserInfoById(Guid userId)
        {
            return _userService.GetUserInfoById(userId)
                .ProjectTo<UserModel>(_mapper.ConfigurationProvider)
                .FirstOrDefault();
        }

        [HttpGet]
        public IEnumerable<MetadataModel> GetUserAttaches()
        {
            Guid id = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return _userService.GetUserAttaches(id)
                .ProjectTo<MetadataModel>(_mapper.ConfigurationProvider)
                .AsEnumerable();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            User user = _mapper.Map<User>(model);
            return await _userService.CreateUser(user);
        }

        [HttpPost]
        public async Task SetUserAvatar(MetadataModel metadata)
        {
            Guid id = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            Avatar avatar = _mapper.Map<Avatar>(metadata);
            await _userService.SetUserAvatar(id, avatar);
        }

        [HttpPost]
        public async Task ChangeFollowStatus(Guid followingId)
        {
            Guid followerId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _userService.ChangeFollowStatus(followerId, followingId);
        }

        [HttpPatch]
        public async Task UpdateUser(UpdateUserModel model)
        {
            Guid id = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            User user = _mapper.Map<User>(model);
            await _userService.UpdateUser(id, user);
        }

        [HttpDelete]
        public async Task DeleteUser()
        {
            Guid id = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _userService.DeleteUser(id);
        }
    }
}
