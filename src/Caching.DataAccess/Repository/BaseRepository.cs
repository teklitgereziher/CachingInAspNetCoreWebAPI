using Caching.DataAccess.Interfaces;
using Caching.DataContract.DbaseContext;

namespace Caching.DataAccess.Repository
{
  public class BaseRepository : IRepository
  {
    private CachingDbContext Context { get; set; }

    public BaseRepository(CachingDbContext context)
    {
      Context = context;
    }

    public IQueryable<TEntity> Query<TEntity>()
      where TEntity : class
    {
      return Context.Set<TEntity>();
    }

    public async Task<TEntity> InsertAsync<TEntity>(
      TEntity entity) where TEntity : class
    {
      await Context.Set<TEntity>().AddAsync(entity);
      await Context.SaveChangesAsync();

      return entity;
    }

    public async Task InsertAsync<TEntity>(
      List<TEntity> entity) where TEntity : class
    {
      await Context.Set<TEntity>().AddRangeAsync(entity);
      await Context.SaveChangesAsync();
    }

    public async Task<TEntity> UpdateAsync<TEntity>(
      TEntity entity) where TEntity : class
    {
      Context.Set<TEntity>().Update(entity);
      await Context.SaveChangesAsync();

      return entity;
    }

    public async Task DeleteAsync<TEntity>(
      TEntity entity) where TEntity : class
    {
      Context.Set<TEntity>().Remove(entity);
      await Context.SaveChangesAsync();
    }
  }
}
