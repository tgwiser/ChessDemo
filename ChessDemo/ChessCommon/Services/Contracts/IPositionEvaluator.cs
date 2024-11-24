using ChessCommon.Models;

namespace ChessCommon.Services.Contracts;

public interface IPositionEvaluator
{
    List<Position> GetLegalPositions(Piece srcPiece);

}
