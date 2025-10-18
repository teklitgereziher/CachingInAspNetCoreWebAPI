using Caching.DataContract.ConfigOptions;
using Caching.DataContract.DbaseContext;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace DataCaching.RestApi.Configurations
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddScopedPostgresSql(
      this IServiceCollection services,
      PostgresSettings postgresSettings,
      bool isDevelopment)
    {
      if (isDevelopment)
      {
        services.AddDbContext<CachingDbContext>(options =>
        {
          options.UseNpgsql(postgresSettings.DevConnectionString);
        }, ServiceLifetime.Scoped);

        return services;
      }

      services.AddDbContext<CachingDbContext>((sp, options) =>
      {
        options.UseNpgsql(postgresSettings.ConnectionString);
        options.AddInterceptors(sp.GetRequiredService<DbConnInterceptor>());
      }, ServiceLifetime.Scoped);

      return services;
    }

    public static IServiceCollection AddSingletonRedis(
      this IServiceCollection services,
      AzureAdSettings adSettings,
      string host,
      bool isDevelopment)
    {
      services.AddSingleton<IConnectionMultiplexer>(sp =>
      {
        if (isDevelopment)
        {
          return ConnectionMultiplexer.Connect(host);
        }
        var configuration = ConfigurationOptions.Parse(host, true)
        .ConfigureForAzureWithServicePrincipalAsync(
          adSettings.ClientId,
          adSettings.TenantId,
          adSettings.ClientSecret).Result;
        return ConnectionMultiplexer.Connect(configuration);
      });
      return services;
    }
  }
}
