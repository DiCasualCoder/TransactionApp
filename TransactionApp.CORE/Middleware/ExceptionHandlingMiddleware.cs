using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using TransactionApp.CORE.CustomException.TransactionException;
using TransactionApp.CORE.Models;

namespace TransactionApp.CORE.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = exception switch
            {
                DomainException domainEx => new ErrorResponse
                {
                    StatusCode = domainEx.StatusCode,
                    Message = domainEx.Message,
                    ErrorType = domainEx.GetType().Name
                },

                ValidationException validationEx => new ErrorResponse
                {
                    StatusCode = 400,
                    Message = "Validation failure",
                    ErrorType = "ValidationError",
                    Errors = new List<string> { validationEx.Message }
                },

                _ => new ErrorResponse
                {
                    StatusCode = 500,
                    Message = "An unexpected error occurred.",
                    ErrorType = "ServerError"
                }
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.StatusCode;
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
