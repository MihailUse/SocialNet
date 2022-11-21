using API.Exceptions;
using API.Models;

namespace API.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IWebHostEnvironment environment)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                if (environment.IsDevelopment())
                    throw;

                ErrorModel errorModel = e switch
                {
                    AuthException => new ErrorModel(e.Message, StatusCodes.Status401Unauthorized),
                    NotFoundServiceException => new ErrorModel(e.Message, StatusCodes.Status404NotFound),
                    AccessDeniedServiceException => new ErrorModel(e.Message, StatusCodes.Status403Forbidden),
                    InvalidParameterServiceException => new ErrorModel(e.Message, StatusCodes.Status422UnprocessableEntity),
                    _ => new ErrorModel("Internal Server Error", StatusCodes.Status500InternalServerError)
                };

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = errorModel.StatusCode;
                await httpContext.Response.WriteAsJsonAsync(errorModel);
                await httpContext.Response.CompleteAsync();
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
