﻿using API.Configs;
using API.Exceptions;
using API.Extentions;
using API.Models.Auth;
using Common.Constants;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Services
{
    public class AuthService
    {
        private readonly DataContext _context;
        private readonly UserService _userService;
        private readonly AuthConfig _authConfig;

        public AuthService(DataContext context, UserService userService, IOptions<AuthConfig> authConfig)
        {
            _context = context;
            _userService = userService;
            _authConfig = authConfig.Value;
        }

        public async Task<TokenModel> GetToken(string login, string password)
        {
            User user = await _userService.GetUserByCredential(login, password);
            UserSession userSession = await CreateUserSession(user.Id);

            return GenerateTokens(userSession);
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

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, parameters, out SecurityToken token);

            if (principal.GetClaimValue<string>(TokenClaimTypes.RefreshTokenId) is String RefreshTokenIdString &&
                Guid.TryParse(RefreshTokenIdString, out Guid refreshTokenId))
            {
                UserSession userSession = await GetUserSessionByRefreshTokenId(refreshTokenId);

                if (!userSession.IsActive)
                    throw new AuthException("Session is not active");

                // update RefreshTokenId
                userSession.RefreshTokenId = Guid.NewGuid();
                await _context.SaveChangesAsync();

                return GenerateTokens(userSession);
            }

            throw new AuthException("Invalid token");
        }

        public async Task<UserSession> GetUserSessionById(Guid sessionId)
        {
            UserSession? userSession = await _context.UserSessions
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == sessionId);

            if (userSession == null)
                throw new AuthException("Session not found");

            return userSession;
        }

        private async Task<UserSession> GetUserSessionByRefreshTokenId(Guid refreshTokenId)
        {
            UserSession? userSession = await _context.UserSessions
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.RefreshTokenId == refreshTokenId);

            if (userSession == null)
                throw new AuthException("Session not found");

            return userSession;
        }

        private async Task<UserSession> CreateUserSession(Guid userId)
        {
            UserSession userSession = new UserSession(userId);
            await _context.UserSessions.AddAsync(userSession);
            await _context.SaveChangesAsync();

            return userSession;
        }

        private TokenModel GenerateTokens(UserSession userSession)
        {
            Claim[] tokenClaims = new Claim[]
            {
                new Claim(TokenClaimTypes.SessionId, userSession.Id.ToString()),
                new Claim(TokenClaimTypes.UserId, userSession.UserId.ToString()),
                new Claim(TokenClaimTypes.Nickname, userSession.User.Nickname)
            };

            Claim[] refreshTokenClaims = new Claim[]
            {
                new Claim(TokenClaimTypes.RefreshTokenId, userSession.RefreshTokenId.ToString()),
            };

            return new TokenModel()
            {
                AccessToken = GenerateEncodedToken(tokenClaims, _authConfig.LifeTime),
                RefreshToken = GenerateEncodedToken(refreshTokenClaims, _authConfig.RefreshLifeTime),
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
