using System.Diagnostics;
using Microsoft.Extensions.Primitives;

namespace DataCaching.RestApi.Middlewares
{
  public sealed class RequestLoggingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private const string CorrelationHeaderName = "X-Correlation-ID";

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
      _next = next ?? throw new ArgumentNullException(nameof(next));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
      ArgumentNullException.ThrowIfNull(context);

      // Ensure a correlation id exists and expose it on the response
      var correlationId = context.Request.Headers.TryGetValue(CorrelationHeaderName, out StringValues value)
        ? value.ToString()
        : context.TraceIdentifier;

      context.Response.Headers[CorrelationHeaderName] = correlationId;

      var requestScope = new Dictionary<string, object>
      {
        ["TraceId"] = context.TraceIdentifier,
        ["CorrelationId"] = correlationId,
        ["RequestMethod"] = context.Request.Method,
        ["RequestPath"] = context.Request.Path.Value,
        ["QueryString"] = context.Request.QueryString.Value,
        ["RemoteIpAddress"] = context.Connection.RemoteIpAddress?.ToString()
      };

      using (_logger.BeginScope(requestScope))
      {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Request starting: {RequestMethod} {RequestPath}{QueryString}", context.Request.Method, context.Request.Path, context.Request.QueryString);

        try
        {
          await _next(context);
        }
        catch (Exception ex)
        {
          // Log exception within same scope so correlation/tracing info is attached
          _logger.LogError(ex, "Unhandled exception while processing request");
          throw;
        }
        finally
        {
          sw.Stop();

          var responseScope = new Dictionary<string, object?>
          {
            ["StatusCode"] = context.Response?.StatusCode,
            ["ElapsedMs"] = sw.ElapsedMilliseconds
          };

          using (_logger.BeginScope(responseScope))
          {
            _logger.LogInformation("Request finished: {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMs}ms",
              context.Request.Method,
              context.Request.Path,
              context.Response?.StatusCode,
              sw.ElapsedMilliseconds);
          }
        }
      }
    }
  }
}
