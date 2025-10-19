using Microsoft.AspNetCore.Mvc;

namespace DataCaching.RestApi.Services.Interfaces
{
  public interface ISeedDataService
  {
    Task<JsonResult> SeedDataAsync();
  }
}
