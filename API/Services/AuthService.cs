using API.Configs;
using API.Models.Auth;
using DAL.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly AuthConfig _authConfig;

        public AuthService(UserService userService, IOptions<AuthConfig> authConfig)
        {
            _userService = userService;
            _authConfig = authConfig.Value;
        }

        public async Task<TokenModel> GetToken(string login, string password)
        {
            User user = await _userService.GetUserByCredential(login, password);
            return GenerateTokens(user);
        }

        public async Task<TokenModel> GetTokensByRefreshToken(string refreshToken)
        {
            TokenValidationParameters parameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _authConfig.SymmetricSecurityKey(),
                ValidAlgorithms = new string[] { SecurityAlgorithms.HmacSha256 },
            };

            // TODO: create self SecurityToken
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, parameters, out SecurityToken token);

            if (principal.FindFirst(x => x.Type == "id")?.Value is String stringId && Guid.TryParse(stringId, out Guid id))
            {
                User user = await _userService.GetUserById(id);
                return GenerateTokens(user);
            }

            throw new Exception("Invalid token");
        }

        private TokenModel GenerateTokens(User user)
        {
            Claim[] JwtClaims = new Claim[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("nickname", user.Nickname)
            };

            Claim[] refreshClaims = new Claim[]
            {
                new Claim("id", user.Id.ToString()),
            };

            return new TokenModel()
            {
                AccessToken = GenerateEncodedToken(JwtClaims, _authConfig.LifeTime),
                RefreshToken = GenerateEncodedToken(refreshClaims, _authConfig.RefreshLifeTime),
            };
        }

        private string GenerateEncodedToken(Claim[] claims, int lifeTime)
        {
            DateTime dateTime = DateTime.UtcNow;

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _authConfig.Issuer,
                audience: _authConfig.Audience,
                claims: claims,
                notBefore: dateTime,
                expires: dateTime.AddMinutes(lifeTime),
                signingCredentials: new SigningCredentials(_authConfig.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
