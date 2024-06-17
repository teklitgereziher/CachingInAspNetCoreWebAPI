using Caching.DataContract.Models;

namespace Caching.DataAccess.Interfaces
{
  public interface IBoardGameRepository
  {
    Task<BoardGame> GetBoardGameAsync(int boardGameId);
    Task<Dictionary<int, BoardGame>> GetBoardGamesDictAsync();
    Task<(List<BoardGame>, int)> GetBoardGamesAsync(
      string filterQuery, int pageIndex, int pageSize,
      string sortColumn, string sortOrder);
    Task<BoardGame> InsertBoardGameAsync(BoardGame boardGame);
    Task InsertBoardGamesAsync(List<BoardGame> boardGames);
    Task<BoardGame> UpdateBoardGameAsync(BoardGame boardGame);
    Task DeleteBoardGameAsync(BoardGame boardGame);
  }
}
