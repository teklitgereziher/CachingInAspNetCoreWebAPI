namespace DataCaching.RestApi.Services.Interfaces
{
  public interface ICacheService
  {
    Task<string> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
  }
}
