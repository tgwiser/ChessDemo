using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Evaluators
{
    public interface IPositionEvaluator
    {
        List<Destination> GetLegalPositions(Piece srcPiece);

        void InitPieces();

    }

}
