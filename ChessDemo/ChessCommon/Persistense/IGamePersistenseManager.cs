using ChessCommon.Models;

namespace ChessCommon.Persistense
{
    public interface IGamePersistenseManager
    {
        Task<Game> GetGame(string fileName);

        Task DeleteGame(string fileName);

        Task UpdateGame(string fileName, string moves);

        Task<List<string>> GetGameNames();

        Task SaveGame(string fileName, string moves);
    }
}