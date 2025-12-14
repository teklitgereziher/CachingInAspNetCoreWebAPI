using DataCaching.RestApi.Models;
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
        if (value == null)
        {
          logger.LogWarning("Board Game with Id: {BoardGameId} was not found.", boardGameId);
          return Problem(
            detail: $"Board Game with Id: {boardGameId} was not found.",
            instance: HttpContext.Request.Path,
            statusCode: 404,
            title: "Board Game Not Found",
            type: "https://httpstatuses.com/404"
            );
        }

        return Ok(value);
      }

      //using (LogContext.PushProperty("MyTraceId", 1234))
      //{
      //  using (LogContext.PushProperty("BoardGameId", boardGameId))
      //  {
      //    logger.LogInformation("Fetching Board Game with Id: {BoardGameId}", boardGameId);
      //    // ...etc
      //  }
      //}
    }

    public override ObjectResult Problem(
      string? detail = null,
      string? instance = null,
      int? statusCode = null,
      string? title = null,
      string? type = null,
      IDictionary<string, object?>? extensions = null)
    {
      var problemDetails = new ErrorResponseDetails
      {
        Detail = detail,
        Instance = instance,
        Status = statusCode ?? 500,
        Title = title,
        Type = type,
      };

      if (extensions is not null)
      {
        foreach (var extension in extensions)
        {
          problemDetails.Extensions.Add(extension);
        }
      }

      return new ObjectResult(problemDetails)
      {
        StatusCode = problemDetails.Status
      };
    }
  }
}
