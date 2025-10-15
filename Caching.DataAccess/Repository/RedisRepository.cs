using Azure.Identity;
using Caching.DataContract.ConfigOptions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Caching.DataAccess.Repository
{
  public class RedisRepository
  {
    // Implementation for Redis repository
    private readonly ClientSecretCredential credential;
    private readonly RedisSettings redisSettings;
    private readonly AzureAdSettings adSettings;
    private readonly IConnectionMultiplexer connectionMultiplexer;

    public RedisRepository(
      ClientSecretCredential clientSecretCredential,
      IOptions<RedisSettings> redisOptions,
      IOptions<AzureAdSettings> adSettings,
      IConnectionMultiplexer connectionMultiplexer)
    {
      credential = clientSecretCredential;
      redisSettings = redisOptions.Value;
      this.adSettings = adSettings.Value;
      this.connectionMultiplexer = connectionMultiplexer;
    }

    public IDatabase GetDatabaseAsync()
    {
      //var options = ConfigurationOptions.Parse(redisSettings.Host);
      //options = options.ConfigureForAzureWithServicePrincipalAsync(
      //  adSettings.ClientId,
      //  adSettings.TenantId,
      //  adSettings.ClientSecret).Result;
      //using var connection = ConnectionMultiplexer.ConnectAsync(options).Result;
      //return connection.GetDatabase();

      return connectionMultiplexer.GetDatabase();
    }
  }
}
