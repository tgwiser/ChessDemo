using ChessCommon.Models;
using ChessCommon.Persistense;
using ChessCommon.Services.Contracts;

namespace ChessCommon.Services;

public class GamePersistenseService : IGamePersistenseService
{
    public IBoardManagerService _boardManager;
    private readonly IChessRepository _chessRepository;

    public GamePersistenseService(IBoardManagerService boardManager, IChessRepository chessRepository)
    {
        _boardManager = boardManager;
        _chessRepository = chessRepository;
    }

    public async Task SaveGame(string fileName, string movesStr)
    {
        await _chessRepository.SaveAsync(new Game() { Moves = movesStr.ToString(), Name = fileName });
    }

    public async Task<Game> GetGame(string fileName)
    {
        var game = await _chessRepository.GetGameAsync(fileName);
        return game;
    }

    public async Task<List<string>> GetGameNames(string filter)
    {
        var games = await _chessRepository.GetGamesNameAsync(filter);
        return games;
    }

    public async Task DeleteGame(string name)
    {
        await _chessRepository.DeleteGame(name);
    }

    public async Task UpdateGame(string name, string moves)
    {
        await _chessRepository.UpdateGame(name, moves);
    }


}
