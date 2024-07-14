using Caching.DataAccess.Interfaces;
using Caching.DataAccess.Repository;
using Caching.DataContract.DbaseContext;
using Caching.Shared.Services;
using Caching.Shared.Services.Interfaces;
using InMemoryCaching.Services;
using InMemoryCaching.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCaching
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      builder.Services.AddMemoryCache();
      builder.Services.AddScoped<IRepository, BaseRepository>();
      builder.Services.AddScoped<ICsvReader, CsvReader>();
      builder.Services.AddScoped<IBoardGameRepository, BoardGameRepository>();
      builder.Services.AddScoped<ISeedDataService, SeedDataService>();
      builder.Services.AddDbContext<CachingDbContext>(options =>
      {
        options.UseNpgsql(
          builder.Configuration.GetConnectionString("DbConnection"));
      }, ServiceLifetime.Scoped);

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
