using Microsoft.AspNetCore.Mvc;

namespace InMemoryCaching.Services.Interfaces
{
  public interface ISeedDataService
  {
    Task<JsonResult> SeedDataAsync();
  }
}
