using ChessCommon.Models;

namespace ChessCommon.Evaluators.Contracts
{

    public interface IBoardManager
    {
        Board? Board { get; set; }

        void DropPiece(Move move);

        void RestorePiece(Move move);

        (bool LeftCastlingEnabled, bool RightCastlingEnabled) GetCastleState(PieceColor color);

        Piece GetPiece(Position position);

        Piece? GetPiece(int y, int x);

        Piece?[,] GetPieces();
    }
}
