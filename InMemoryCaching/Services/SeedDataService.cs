using Caching.DataAccess.Interfaces;
using Caching.DataContract.Models;
using Caching.Shared.Services.Interfaces;
using InMemoryCaching.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InMemoryCaching.Services
{
  public class SeedDataService : ISeedDataService
  {
    private IBoardGameRepository _boardGameRepo;
    private readonly ICsvReader _csvDataReader;

    public SeedDataService(
      IBoardGameRepository repository,
      ICsvReader csvDataReader)
    {
      _boardGameRepo = repository;
      _csvDataReader = csvDataReader;
    }

    public async Task<JsonResult> SeedDataAsync()
    {
      var existingBoardGames = await _boardGameRepo.GetBoardGamesDictAsync();
      var now = DateTime.UtcNow;
      var games = new List<BoardGame>();
      var records = _csvDataReader.Read();

      var skippedRows = 0;
      foreach (var record in records)
      {
        if (!record.ID.HasValue
        || string.IsNullOrEmpty(record.Name)
        || existingBoardGames.ContainsKey(record.ID.Value))
        {
          skippedRows++;
          continue;
        }
        var boardgame = new BoardGame()
        {
          BoardGameId = record.ID.Value,
          Name = record.Name,
          BGGRank = record.BGGRank ?? 0,
          ComplexityAverage = record.ComplexityAverage ?? 0,
          MaxPlayers = record.MaxPlayers ?? 0,
          MinAge = record.MinAge ?? 0,
          MinPlayers = record.MinPlayers ?? 0,
          PlayTime = record.PlayTime ?? 0,
          RatingAverage = record.RatingAverage ?? 0,
          UsersRated = record.UsersRated ?? 0,
          Year = record.YearPublished ?? 0
        };
        games.Add(boardgame);
      }

      await _boardGameRepo.InsertBoardGamesAsync(games);

      return new JsonResult(new
      {
        BoardGames = games.Count(),
        SkippedRows = skippedRows
      });
    }
  }
}
