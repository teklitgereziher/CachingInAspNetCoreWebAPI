using DataCaching.RestApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataCaching.RestApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BoardGamesController : ControllerBase
  {
    private readonly IGameService gameService;

    public BoardGamesController(IGameService gameService)
    {
      this.gameService = gameService;
    }

    [HttpGet("{boardGameId}")]
    public async Task<IActionResult> GetBoardGame(int boardGameId)
    {
      var value = await gameService.GetBoardGameAsync(boardGameId);
      return Ok(value);
    }
  }
}
