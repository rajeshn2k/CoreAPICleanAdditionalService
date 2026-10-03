using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.API.Clean.AdditionalService
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing request");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("The response has already started, the global exception handler will not be able to write the response.");
                throw exception;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/json";

            // Map unexpected exceptions to Service Unavailable to match prior controller behavior
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.ServiceUnavailable;

            var correlationId = context.GetCorrelationId();

            // Build ApiErrorResponse from shared library
            var errorResponse = Core.Library.Clean.AdditionalService.ApiErrorResponse.CreateError(
                Core.Library.Clean.AdditionalService.ErrorCodes.INTERNAL_ERROR,
                "An unexpected error occurred.",
                (int)System.Net.HttpStatusCode.ServiceUnavailable,
                correlationId,
                context.Request.Path,
                _env.IsDevelopment() ? exception.ToString() : null);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var payload = JsonSerializer.Serialize(errorResponse, options);

            return context.Response.WriteAsync(payload);
        }
    }
}
