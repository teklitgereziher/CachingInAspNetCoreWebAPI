using Caching.Shared.Models;

namespace Caching.Shared.Services.Interfaces
{
  public interface ICsvReader
  {
    IEnumerable<BggRecord> Read();
  }
}
