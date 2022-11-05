using API.Models.Auth;
using API.Services;
using DAL.Entities;
using System.Net;

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
            string? sessionIdString = httpContext.User.FindFirst(x => x.Type == TokenClaimTypes.SessionId)?.Value;
            string? RefreshTokenIdString = httpContext.User.FindFirst(x => x.Type == RefreshTokenClaimTypes.RefreshTokenId)?.Value;

            if (RefreshTokenIdString != null)
            {
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpContext.Response.WriteAsync("Invalid token");
                return;
            }

            if (Guid.TryParse(sessionIdString, out Guid sessionId))
            {
                UserSession userSession = await authService.GetUserSessionById(sessionId);

                if (!userSession.IsActive)
                {
                    httpContext.Response.Clear();
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await httpContext.Response.WriteAsync("Invalid token");
                    return;
                }
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
