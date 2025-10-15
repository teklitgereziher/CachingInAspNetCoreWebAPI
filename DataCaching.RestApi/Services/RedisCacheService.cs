using Azure.Identity;
using DataCaching.RestApi.Services.Interfaces;
using StackExchange.Redis;

namespace DataCaching.RestApi.Services
{
  public class RedisCacheService : ICacheService
  {
    private readonly IDatabase cache;
    private readonly ILogger<RedisCacheService> logger;
    private readonly ClientSecretCredential credential;
    private readonly IConnectionMultiplexer connectionMultiplexer;

    public RedisCacheService(
      ILogger<RedisCacheService> logger,
      IConnectionMultiplexer connectionMultiplexer)
    {
      this.logger = logger;
      cache = connectionMultiplexer.GetDatabase();
    }

    public async Task<string> GetValueAsync(string key)
    {
      return await cache.StringGetAsync(key);
    }

    public async Task SetValueAsync(string key, string value)
    {
      await cache.StringSetAsync(key, value);
    }
  }
}
