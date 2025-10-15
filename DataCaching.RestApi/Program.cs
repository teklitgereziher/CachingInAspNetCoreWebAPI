using Azure.Identity;
using Caching.DataAccess.Interfaces;
using Caching.DataAccess.Repository;
using Caching.DataContract.ConfigOptions;
using Caching.DataContract.DbaseContext;
using Caching.Shared.Services;
using Caching.Shared.Services.Interfaces;
using DataCaching.RestApi.Configurations;
using DataCaching.RestApi.Services;
using DataCaching.RestApi.Services.Interfaces;
using StackExchange.Redis;

namespace DataCaching.RestApi
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      var adSettings = builder.Configuration.GetSection("AzureAdSettings").Get<AzureAdSettings>();
      var pgSqlSettings = builder.Configuration.GetSection("PostgresSettings").Get<PostgresSettings>();
      builder.Services.Configure<AzureAdSettings>(builder.Configuration.GetSection("AzureAdSettings"));
      builder.Services.Configure<PostgresSettings>(builder.Configuration.GetSection("PostgresSettings"));
      builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
      builder.Services.AddMemoryCache();
      builder.Services.AddSingleton(new ClientSecretCredential(
        adSettings.TenantId,
        adSettings.ClientId,
        adSettings.ClientSecret));
      // Register Redis cache
      builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
      {
        var configuration = ConfigurationOptions.Parse(
          builder.Configuration.GetConnectionString("Redis"), true)
        .ConfigureForAzureWithServicePrincipalAsync(
          adSettings.ClientId,
          adSettings.TenantId,
          adSettings.ClientSecret).Result;
        return ConnectionMultiplexer.Connect(configuration);
      });

      builder.Services.AddScoped<RedisRepository>();
      builder.Services.AddScoped<DbConnInterceptor>();
      builder.Services.AddScoped<ICacheService, RedisCacheService>();
      builder.Services.AddScoped<IRepository, BaseRepository>();
      builder.Services.AddScoped<ICsvReader, CsvReader>();
      builder.Services.AddScoped<IBoardGameRepository, BoardGameRepository>();
      builder.Services.AddScoped<ISeedDataService, SeedDataService>();
      builder.Services.AddScopedPostgresSql(pgSqlSettings, builder.Environment.IsDevelopment());

      builder.Services.AddControllers();
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen();

      var app = builder.Build();

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI();
      }

      app.UseHttpsRedirection();

      app.UseAuthorization();


      app.MapControllers();

      app.Run();
    }
  }
}
