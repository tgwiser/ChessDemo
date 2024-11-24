using ChessCommon.Models;

namespace ChessCommon.Services.Contracts
{
    public interface IGamePersistenseService
    {
        Task<Game> GetGame(string fileName);

        Task DeleteGame(string fileName);

        Task UpdateGame(string fileName, string moves);

        Task<List<string>> GetGameNames(string filter);

        Task SaveGame(string fileName, string moves);
    }
}