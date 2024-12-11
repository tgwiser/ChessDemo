using ChessCommon.Models;

namespace ChessCommon.Services.Contracts
{
    public interface IGameHistoryService
    {
        void AddMove(Move move);

        bool TryGetNextMove(out Move? move);

        bool TryGetPrevMove(out Move? move);

        List<Move> GetMoves();

        void Reset();
    }
}
