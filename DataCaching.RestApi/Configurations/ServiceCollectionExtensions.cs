using Caching.DataContract.ConfigOptions;
using Caching.DataContract.DbaseContext;
using Microsoft.EntityFrameworkCore;

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
  }
}
