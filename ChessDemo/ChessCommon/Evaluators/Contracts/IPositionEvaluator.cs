using ChessCommon.Models;

namespace ChessCommon.Evaluators.Contracts;

public interface IPositionEvaluator
{
    List<Position> GetLegalPositions(Piece srcPiece);

}
