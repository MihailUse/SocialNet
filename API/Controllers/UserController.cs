using API.Extentions;
using API.Models.Attach;
using API.Models.User;
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
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService, ProjectionGeneratorService projectionGeneratorService)
        {
            _userService = userService;

            projectionGeneratorService.AvatarLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new { userId = x.UserId });
            projectionGeneratorService.AttachLinkGenerator =
                x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAttach), new { attachId = x.Id });
        }

        // for testing
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<UserModel> GetUsers()
        {
            return _userService.GetUsers();
        }

        [HttpGet]
        public IEnumerable<MetadataModel> GetUserAttaches()
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return _userService.GetUserAttaches(userId);
        }

        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<SearchListUserModel> SearchUsers(string? search, int skip = 0, int take = 20)
        {
            return _userService.SearchUsers(search ?? string.Empty, skip, take);
        }

        [HttpGet]
        public async Task<UserProfileModel> GetUserProfile(Guid userId)
        {
            return await _userService.GetUserProfile(userId);
        }

        [HttpGet]
        public async Task<UserProfileModel> GetCurrentUserProfile()
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return await _userService.GetUserProfile(userId);
        }

        [HttpPost]
        public async Task SetUserAvatar(MetadataModel metadata)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _userService.SetUserAvatar(userId, metadata);
        }

        [HttpPost]
        public async Task ChangeFollowStatus(Guid followingId)
        {
            Guid followerId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _userService.ChangeFollowStatus(followerId, followingId);
        }

        [HttpPatch]
        public async Task UpdateUser(UpdateUserModel updateModel)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _userService.UpdateUser(userId, updateModel);
        }

        [HttpDelete]
        public async Task DeleteUser()
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _userService.DeleteUser(userId);
        }
    }
}
