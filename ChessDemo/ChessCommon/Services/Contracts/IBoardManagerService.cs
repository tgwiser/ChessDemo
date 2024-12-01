using ChessCommon.Models;

namespace ChessCommon.Services.Contracts
{

    public interface IBoardManagerService
    {
        Board? Board { get; set; }

        void DropPiece(Move move);

        void RestorePiece(Move move);

        (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) GetCastleState(PieceColor color);


        Piece GetPiece(Position position);

        Piece? GetPiece(int y, int x);

        Piece?[,] GetPieces();
    }
}
