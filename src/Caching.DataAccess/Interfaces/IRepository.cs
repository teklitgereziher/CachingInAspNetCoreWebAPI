namespace Caching.DataAccess.Interfaces
{
  public interface IRepository
  {
    IQueryable<TEntity> Query<TEntity>() where TEntity : class;
    Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class;
    Task InsertAsync<TEntity>(List<TEntity> entities) where TEntity : class;
    Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
    Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;
  }
}
