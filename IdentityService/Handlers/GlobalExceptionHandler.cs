using System.Net;
using Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, message) = GetExceptionDetails(exception);
            _logger.LogError(exception, exception.Message);
            httpContext.Response.StatusCode = (int)statusCode;
            await httpContext.Response.WriteAsJsonAsync(message, cancellationToken);

            return true;
        }

        private (HttpStatusCode statusCode, string message) GetExceptionDetails(Exception exception)
        {
            return exception switch
            {
                EmptyEmailException => (HttpStatusCode.BadRequest, exception.Message),
                EmptyNameException => (HttpStatusCode.BadRequest, exception.Message),
                LoginFailException => (HttpStatusCode.Unauthorized, exception.Message),
                UserAlreadyExistException => (HttpStatusCode.Conflict, exception.Message),
                RegistrationFailException => (HttpStatusCode.BadRequest, exception.Message),
                RefreshTokenException => (HttpStatusCode.Unauthorized, exception.Message),
                _ => (HttpStatusCode.InternalServerError, $"Có lỗi xảy ra: {exception.Message}")
            };
        }
    }
}