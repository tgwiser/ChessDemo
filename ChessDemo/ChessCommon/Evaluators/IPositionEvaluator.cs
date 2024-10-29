using ChessCommon.Models;

namespace ChessCommon.Evaluators;

public interface IPositionEvaluator
{
    List<Destination> GetLegalPositions(Piece srcPiece);

    void InitPieces();
}
