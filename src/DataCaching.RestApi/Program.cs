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
using Serilog;

namespace DataCaching.RestApi
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

      // Add services to the container.
      var adSettings = builder.Configuration.GetSection("AzureAdSettings").Get<AzureAdSettings>();
      var pgSqlSettings = builder.Configuration.GetSection("PostgresSettings").Get<PostgresSettings>();
      builder.Services.Configure<AzureAdSettings>(builder.Configuration.GetSection("AzureAdSettings"));
      builder.Services.Configure<PostgresSettings>(builder.Configuration.GetSection("PostgresSettings"));
      builder.Services.AddMemoryCache();
      builder.Services.AddSingleton(new ClientSecretCredential(
        adSettings.TenantId,
        adSettings.ClientId,
        adSettings.ClientSecret));
      // Register Redis cache
      builder.Services.AddSingletonRedis(
        adSettings,
        builder.Configuration.GetSection("Redis:Host").Get<string>(),
        builder.Environment.IsDevelopment());

      builder.Services.AddScoped<IRedisRepository, RedisRepository>();
      builder.Services.AddScoped<DbConnInterceptor>();
      builder.Services.AddScoped<IGameService, GameService>();
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
