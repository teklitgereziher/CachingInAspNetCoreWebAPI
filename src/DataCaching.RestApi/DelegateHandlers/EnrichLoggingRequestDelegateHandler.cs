namespace DataCaching.RestApi.DelegateHandlers
{
  public class EnrichLoggingRequestDelegateHandler : DelegatingHandler
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CorrelationHeaderName = "X-Correlation-ID";

    public EnrichLoggingRequestDelegateHandler(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      var context = _httpContextAccessor.HttpContext;
      if (context != null && context.Request.Headers.TryGetValue(CorrelationHeaderName, out var correlationId))
      {
        // Add the correlation ID from the incoming HTTP request to the outgoing HTTP request headers
        request.Headers.Add(CorrelationHeaderName, correlationId.ToString());
      }
      return await base.SendAsync(request, cancellationToken);
    }
  }
}
