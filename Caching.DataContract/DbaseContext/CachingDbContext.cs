using Caching.DataContract.Models;
using Microsoft.EntityFrameworkCore;

namespace Caching.DataContract.DbaseContext
{
  public class CachingDbContext : DbContext
  {
    public DbSet<BoardGame> BoardGames => Set<BoardGame>();

    public CachingDbContext(DbContextOptions<CachingDbContext> options)
      : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }
  }
}
