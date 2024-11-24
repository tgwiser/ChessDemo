using ChessCommon.Evaluators.Contracts;
using ChessCommon.Models;
using System.Text;

namespace ChessCommon.Persistense;

public class GamePersistenseManager: IGamePersistenseManager
{
    /// <summary>
    /// Board y-dimension
    /// </summary>
    public const int MAX_ROWS = 8;

    /// <summary>
    /// Board x-dimension
    /// </summary>
    public const int MAX_COLS = 8;

    public IBoardManager _boardManager;
    private readonly IChessRepository _chessRepository;

    public GamePersistenseManager(IBoardManager boardManager, IChessRepository chessRepository)
    {
        _boardManager = boardManager;
        _chessRepository = chessRepository;
    }

    public async Task SaveGame(string fileName,string movesStr)
    {
        await _chessRepository.SaveAsync(new Game() { Moves = movesStr.ToString(), Name = fileName});
    }

    public async Task<Game> GetGame(string fileName)
    {
        var game = await _chessRepository.GetGameAsync(fileName);
        return game;
    }

    public async Task<List<string>> GetGameNames()
    {
        var games = await _chessRepository.GetGamesNameAsync();
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
