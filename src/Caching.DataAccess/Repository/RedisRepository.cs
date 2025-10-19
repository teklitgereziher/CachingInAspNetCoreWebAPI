using Caching.DataAccess.Interfaces;
using StackExchange.Redis;

namespace Caching.DataAccess.Repository
{
  public class RedisRepository(
    IConnectionMultiplexer connectionMultiplexer
    ) : IRedisRepository
  {
    private readonly IDatabase database = connectionMultiplexer.GetDatabase();

    public async Task<string> GetValueAsync(string key)
    {
      return await database.StringGetAsync(key);
    }

    public async Task SetValueAsync(string key, string value)
    {
      await database.StringSetAsync(key, value);
    }
  }
}
