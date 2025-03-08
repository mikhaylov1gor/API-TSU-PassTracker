using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using API_TSU_PassTracker.Models.DTO;
using API_TSU_PassTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

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
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var tokenBlackListService = scope.ServiceProvider.GetRequiredService<ITokenBlackListService>();

                    var token = context.Request.Headers["Authorization"].ToString()?.Split(" ").Last();
                    if (!string.IsNullOrEmpty(token) && await tokenBlackListService.iSTokenRevoked(token))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var response = new ErrorResponse(
                            status: 401,
                            message: "Токен не валиден",
                            details: null
                        );
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        return;
                    }
                }

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

            
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Ответ уже начал отправляться, обработка ошибки невозможна.");
                return;
            }

            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            
            var response = new ErrorResponse(
                status: context.Response.StatusCode,
                message: "Внутренняя ошибка сервера",
                details: _env.IsDevelopment() ? ex.ToString() : null
            );

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