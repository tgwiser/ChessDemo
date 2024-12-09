using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
