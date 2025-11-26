using System.Diagnostics;
using Microsoft.Extensions.Primitives;

namespace DataCaching.RestApi.Middlewares
{
  public class CorrelationContextMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private const string CorrelationHeaderName = "X-Correlation-ID";

    // ActivitySource for built-in tracing (consumers/listeners can pick this up)
    private static readonly ActivitySource ActivitySource = new("DataCaching.RestApi.Request");

    public CorrelationContextMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
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
        // Start an Activity (built-in tracing) instead of Stopwatch.
        // Activity.Duration will be available after the Activity is stopped.
        using var activity = ActivitySource.StartActivity("http.server.request", ActivityKind.Server);

        if (activity is not null)
        {
          activity.SetTag("http.method", context.Request.Method);
          activity.SetTag("http.target", context.Request.Path + context.Request.QueryString);
          activity.SetTag("http.client_ip", context.Connection.RemoteIpAddress?.ToString());
          activity.SetTag("request.id", context.TraceIdentifier);
          activity.SetTag("correlation_id", correlationId);
        }

        _logger.LogInformation("Request starting: {RequestMethod} {RequestPath}{QueryString}", context.Request.Method, context.Request.Path, context.Request.QueryString);

        try
        {
          await _next(context);
        }
        catch (Exception ex)
        {
          // Record exception information on the Activity so tracing systems can correlate it
          if (activity is not null)
          {
            activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity.SetTag("otel.status_code", "ERROR");
            activity.SetTag("error", true);
            // RecordException is available on Activity in recent runtimes; guard usage
            //try { activity.RecordException(ex); } catch { /* ignore if unavailable */ }
          }

          // Log exception within same scope so correlation/tracing info is attached
          _logger.LogError(ex, "Unhandled exception while processing request");
          throw;
        }
        finally
        {
          // Stop the activity so Duration is populated
          activity?.Stop();

          var elapsedMs = activity?.Duration.TotalMilliseconds ?? -1.0;

          var responseScope = new Dictionary<string, object?>
          {
            ["StatusCode"] = context.Response?.StatusCode,
            ["ElapsedMs"] = elapsedMs
          };

          using (_logger.BeginScope(responseScope))
          {
            _logger.LogInformation("Request finished: {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMs}ms",
              context.Request.Method,
              context.Request.Path,
              context.Response?.StatusCode,
              elapsedMs);
          }
        }
      }
    }
  }
}
