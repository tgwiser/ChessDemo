using ChessCommon.Models;

namespace ChessCommon.Persistense
{
    public interface IGamePersistenseManager
    {
        Board? GetBoard(string fileName);
        Task SaveBoard(string fileName);
    }
}