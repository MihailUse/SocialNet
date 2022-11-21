using API.Models.Auth;
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

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<TokenModel> GetToken(RequestTokenModel model) => await _authService.GetToken(model.Email, model.Password);

        [HttpPost]
        public Task<TokenModel> GetRefreshToken(RequestRefreshTokenModel model) => _authService.GetTokensByRefreshToken(model.RefreshToken);
    }
}
