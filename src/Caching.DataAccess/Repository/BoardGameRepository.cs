using Caching.DataAccess.Interfaces;
using Caching.DataContract.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Caching.DataAccess.Repository
{
  public class BoardGameRepository : IBoardGameRepository
  {
    private readonly IRepository _repository;

    public BoardGameRepository(IRepository repository)
    {
      _repository = repository;
    }

    public async Task<BoardGame> GetBoardGameAsync(int boardGameId)
    {
      return await _repository
        .Query<BoardGame>()
        .FirstOrDefaultAsync(b => b.BoardGameId == boardGameId);
    }

    public async Task<Dictionary<int, BoardGame>> GetBoardGamesDictAsync()
    {
      return await _repository
        .Query<BoardGame>()
        .ToDictionaryAsync(b => b.BoardGameId);
    }

    public async Task<(List<BoardGame>, int)> GetBoardGamesAsync(
      string filterQuery,
      int pageIndex,
      int pageSize,
      string sortColumn,
      string sortOrder)
    {
      var games = _repository
        .Query<BoardGame>()
        .OrderBy($"{sortColumn} {sortOrder}")
        .AsQueryable();

      if (!string.IsNullOrEmpty(filterQuery))
      {
        games = games.Where(b => b.Name.Contains(filterQuery));
      }

      var gameCount = games.Count();
      var gamesList = await games
        .Skip(pageIndex * pageSize)
        .Take(pageSize)
        .ToListAsync();

      return (gamesList, gameCount);
    }

    public async Task<BoardGame> InsertBoardGameAsync(BoardGame boardGame)
    {
      return await _repository.InsertAsync(boardGame);
    }

    public async Task InsertBoardGamesAsync(List<BoardGame> boardGames)
    {
      await _repository.InsertAsync(boardGames);
    }

    public async Task<BoardGame> UpdateBoardGameAsync(BoardGame boardGame)
    {
      return await _repository.UpdateAsync(boardGame);
    }

    public async Task DeleteBoardGameAsync(BoardGame boardGame)
    {
      await _repository.DeleteAsync(boardGame);
    }
  }
}
