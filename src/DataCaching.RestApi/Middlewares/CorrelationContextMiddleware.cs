using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace DataCaching.RestApi.Middlewares
{
  public sealed class CorrelationContextMiddleware(RequestDelegate next, ILogger<CorrelationContextMiddleware> logger)
  {
    private readonly RequestDelegate _next = next;
    private readonly ILogger<CorrelationContextMiddleware> _logger = logger;
    private const string CorrelationHeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
      // Ensure a correlation id exists and expose it on the response
      var correlationId = context.Request.Headers.TryGetValue(CorrelationHeaderName, out StringValues value)
        ? value.ToString()
        : context.TraceIdentifier;

      context.Request.Headers[CorrelationHeaderName] = correlationId;
      context.Response.Headers[CorrelationHeaderName] = correlationId;

      using (LogContext.PushProperty("CorrelationId", correlationId))
      {
        _logger.LogInformation("Handling request: {Method} {Url} CorrelationId: {CorrelationId}",
          context.Request.Method,
          context.Request.Path + context.Request.QueryString,
          correlationId);
        await _next(context);
        _logger.LogInformation("Finished handling request. CorrelationId: {CorrelationId}", correlationId);
      }
    }
  }
}
