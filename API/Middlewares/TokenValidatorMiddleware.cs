using API.Exceptions;
using API.Services;
using Common.Constants;
using Common.Extentions;
using DAL.Entities;

namespace API.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TokenValidatorMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, AuthService authService)
        {
            Guid sessionId = httpContext.User.GetClaimValue<Guid>(TokenClaimTypes.SessionId, false);
            Guid RefreshTokenId = httpContext.User.GetClaimValue<Guid>(TokenClaimTypes.RefreshTokenId, false);

            // check it's not a refresh token
            if (RefreshTokenId != default)
                throw new AuthException("Invalid token");


            if (sessionId != default)
            {
                UserSession userSession = await authService.GetUserSessionById(sessionId);

                if (!userSession.IsActive)
                    throw new AuthException("Invalid token");
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class TokenValidatorMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidatorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidatorMiddleware>();
        }
    }
}
