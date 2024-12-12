using ChessCommon.Models;

namespace ChessCommon.Persistense
{
    public interface IChessRepository
    {
        Task<List<string>> GetGamesNameAsync();

        Task<List<string>> GetGamesNameAsync(string filter);

        Task<Game> GetGameAsync(string name);

        Task SaveAsync(Game game);

        Task DeleteGame(string name);

        Task UpdateGame(string name, string moves);
    }
}
