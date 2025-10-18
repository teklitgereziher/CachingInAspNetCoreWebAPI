using Caching.DataContract.Models;

namespace DataCaching.RestApi.Services.Interfaces
{
  public interface IGameService
  {
    Task<BoardGame> GetBoardGameAsync(int boardGameId);
  }
}
