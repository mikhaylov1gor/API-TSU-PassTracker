using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Hosting;
using API_TSU_PassTracker.Models.DTO;

namespace API_TSU_PassTracker.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;

        public CustomExceptionFilter(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            var errorResponse = context.Exception switch
            {
                ArgumentException argEx => new ErrorResponse(
                    StatusCodes.Status400BadRequest,
                    argEx.Message,
                    _env.IsDevelopment() ? context.Exception.StackTrace : null
                ),
                UnauthorizedAccessException => new ErrorResponse(
                    StatusCodes.Status401Unauthorized,
                    "Доступ запрещен",
                    _env.IsDevelopment() ? context.Exception.ToString() : null
                ),
                _ => new ErrorResponse(
                    StatusCodes.Status500InternalServerError,
                    "Внутренняя ошибка сервера",
                    _env.IsDevelopment() ? context.Exception.ToString() : null
                )
            };

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = errorResponse.Status
            };
            context.ExceptionHandled = true; 
        }
    }
}
