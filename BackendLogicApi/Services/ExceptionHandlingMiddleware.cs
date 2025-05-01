using static BackendLogicApi.Services.AuthService;
using System.Net;

namespace BackendLogicApi.Services
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); // Przechodzi do następnego middleware w pipeline
            }
            catch (ConflictException ex)
            {
                _logger.LogError(ex, "Konflikt: {0}", ex.Message);
                httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict; // 409 dla konfliktu
                await httpContext.Response.WriteAsync(ex.Message); // Zwróć szczegóły konfliktu
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił nieoczekiwany błąd.");
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500 dla ogólnych błędów
                await httpContext.Response.WriteAsync("Wystąpił błąd serwera.");
            }
        }
    }

}
