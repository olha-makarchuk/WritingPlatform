using CorrelationId.Abstractions;
using Newtonsoft.Json;

namespace WritingPlatformApi.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public GlobalExceptionMiddleware(RequestDelegate next, ICorrelationContextAccessor correlationContextAccessor)
        {
            _next = next;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public async Task Invoke(HttpContext context, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, env,
                    _correlationContextAccessor.CorrelationContext.CorrelationId, loggerFactory.CreateLogger(ex.Source));
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception,
            IWebHostEnvironment env, string correlationId, ILogger logger)
        {
            var code = StatusCodes.Status500InternalServerError;
            string prodMessage = string.Empty;
            string correlationIdMessage = $"Request CorrelationId is '{correlationId}'.";

            logger?.LogError(exception, $"{correlationIdMessage}:{exception.Message}");

            var result = env.IsDevelopment() ?
                JsonConvert.SerializeObject(new { error = exception.Message, correlationId, stackTrace = exception.StackTrace }, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) :
                JsonConvert.SerializeObject(new { error = $"Error occured while processing your request. {correlationIdMessage}{prodMessage}" });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
            return context.Response.WriteAsync(result);
        }
    }
}
