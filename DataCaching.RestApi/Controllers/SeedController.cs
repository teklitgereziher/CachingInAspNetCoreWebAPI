using DataCaching.RestApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataCaching.RestApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SeedController : ControllerBase
  {
    private readonly ISeedDataService _seedService;
    private readonly ILogger<SeedController> _logger;

    public SeedController(
      ISeedDataService seedService,
      ILogger<SeedController> logger)
    {
      _logger = logger;
      _seedService = seedService;
    }

    [HttpPut(Name = "Seed")]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> SeedBoardGames()
    {
      return await _seedService.SeedDataAsync();
    }
  }
}
