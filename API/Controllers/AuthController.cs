using API.Models.Auth;
using API.Models.User;
using API.Services;
using Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = SwaggerDefinitionNames.Auth)]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public AuthController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<TokenModel> GetToken(RequestTokenModel model) => await _authService.GetToken(model.Email, model.Password);

        [HttpPost]
        public async Task<TokenModel> GetRefreshToken(RequestRefreshTokenModel model) => await _authService.GetTokensByRefreshToken(model.RefreshToken);

        [HttpPost]
        public async Task<TokenModel> CreateUser(CreateUserModel createModel)
        {
            await _userService.CreateUser(createModel);
            return await _authService.GetToken(createModel.Email, createModel.Password);
        }
    }
}
