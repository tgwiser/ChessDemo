using ChessCommon.Models;

namespace ChessCommon.Evaluators;

public interface IPositionEvaluator
{
    List<Position> GetLegalPositions(Piece srcPiece);

    void InitPieces();
}
