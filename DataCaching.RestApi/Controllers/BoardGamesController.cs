using DataCaching.RestApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataCaching.RestApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BoardGamesController : ControllerBase
  {
    private readonly ICacheService cacheService;

    public BoardGamesController(ICacheService cacheService)
    {
      this.cacheService = cacheService;
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key)
    {
      var value = await cacheService.GetValueAsync(key);
      return Ok(value);
    }

    [HttpPost]
    public async Task<IActionResult> Set([FromQuery] string key, [FromBody] string value)
    {
      await cacheService.SetValueAsync(key, value);
      return Ok();
    }
  }
}
