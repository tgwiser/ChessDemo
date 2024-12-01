using ChessCommon.Models;

namespace ChessCommon.Services.Contracts;

public interface IPositionEvaluatorService
{
    List<Position> GetLegalPositions(Piece srcPiece);

}
