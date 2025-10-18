namespace Caching.DataAccess.Interfaces
{
  public interface IRedisRepository
  {
    Task<string> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
  }
}
