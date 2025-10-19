using Caching.DataAccess.Interfaces;
using Caching.DataContract.Models;
using DataCaching.RestApi.Services.Interfaces;
using Newtonsoft.Json;

namespace DataCaching.RestApi.Services
{
  public class GameService : IGameService
  {
    private readonly ILogger<GameService> logger;
    private readonly IBoardGameRepository gameRepository;
    private readonly IRedisRepository cache;

    public GameService(
      ILogger<GameService> logger,
      IBoardGameRepository gameRepository,
      IRedisRepository redisRepository)
    {
      this.logger = logger;
      this.gameRepository = gameRepository;
      cache = redisRepository;
    }

    public async Task<BoardGame> GetBoardGameAsync(int boardGameId)
    {
      var cacheValue = await cache.GetValueAsync(boardGameId.ToString());
      if (cacheValue != null)
      {
        logger.LogInformation("Cache hit for BoardGameId: {BoardGameId}", boardGameId);
        return JsonConvert.DeserializeObject<BoardGame>(cacheValue);
      }

      logger.LogInformation("Cache miss for BoardGameId: {BoardGameId}", boardGameId);
      var boardGame = await gameRepository.GetBoardGameAsync(boardGameId);
      if (boardGame != null)
      {
        var serializedGame = JsonConvert.SerializeObject(boardGame);
        await cache.SetValueAsync(boardGameId.ToString(), serializedGame);
      }
      return boardGame;
    }
  }
}
