using DataCaching.RestApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataCaching.RestApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BoardGamesController : ControllerBase
  {
    private readonly IGameService gameService;
    private readonly ILogger<BoardGamesController> logger;

    public BoardGamesController(
      IGameService gameService,
      ILogger<BoardGamesController> logger)
    {
      this.gameService = gameService;
      this.logger = logger;
    }

    [HttpGet("{boardGameId}")]
    public async Task<IActionResult> GetBoardGame(int boardGameId)
    {
      using (logger.BeginScope(new Dictionary<string, object> {
        { "MyTraceId", "12345"},
        { "GameId", boardGameId.ToString() }
      }))
      {
        logger.LogInformation("Fetching Board Game with Id: {BoardGameId}", boardGameId);
        var value = await gameService.GetBoardGameAsync(boardGameId);
        return Ok(value);
      }
    }
  }
}
