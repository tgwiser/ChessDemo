using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Evaluators
{

    public interface IBoardManager
    {
        Board? Board { get;  set; }

        void DropPiece(Move move);

        void RestorePiece(Move move);

        (bool, bool) GetCastleState(Position position);
        Piece GetPiece(Position position);
    }
}
