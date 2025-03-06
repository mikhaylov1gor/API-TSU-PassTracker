using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using API_TSU_PassTracker.Models.DTO;
using API_TSU_PassTracker.Services;
using Microsoft.Extensions.DependencyInjection;

namespace API_TSU_PassTracker.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IServiceProvider _serviceProvider;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            _next = next;
            _logger = logger;
            _env = env;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tokenBlackListService = scope.ServiceProvider.GetRequiredService<ITokenBlackListService>();

                var token = context.Request.Headers["Authorization"].ToString()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token) && await tokenBlackListService.iSTokenRevoked(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token is revoked");
                    return;
                }
            }

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "Произошла необработанная ошибка");

            var response = new ErrorResponse(
                status: StatusCodes.Status500InternalServerError,
                message: "Внутренняя ошибка сервера",
                details: _env.IsDevelopment() ? ex.ToString() : null
            );

            context.Response.StatusCode = response.Status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}