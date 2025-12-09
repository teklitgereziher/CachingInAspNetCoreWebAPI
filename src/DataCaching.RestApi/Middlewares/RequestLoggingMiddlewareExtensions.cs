namespace DataCaching.RestApi.Middlewares
{
  /// <summary>
  /// Provides extension methods for adding request logging middleware to a <see cref="WebApplication"/>.
  /// Register the middleware in the request pipeline (place before app.MapControllers() or before other terminal middleware).
  /// </summary>
  public static class RequestLoggingMiddlewareExtensions
  {
    // WebApplication fluent extension for minimal hosting
    public static WebApplication UseCustomRequestLogging(this WebApplication app)
    {
      app.UseMiddleware<CorrelationContextMiddleware>();
      return app;
    }
  }
}
