namespace Caching.DataContract.ConfigOptions
{
  public class PostgresSettings
  {
    public string[] Scopes { get; set; }
    public string ConnectionString { get; set; }
    public string DevConnectionString { get; set; }
  }
}
