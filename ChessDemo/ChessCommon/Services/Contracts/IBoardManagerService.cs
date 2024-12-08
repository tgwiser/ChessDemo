using ChessCommon.Models;

namespace ChessCommon.Services.Contracts
{

    public interface IBoardManagerService
    {
        Board? Board { get;  }

        void DropPiece(Move move);

        void RestorePiece(Move move);

        (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) GetCastleState(PieceColor color);


        Piece GetPiece(Position position);

        Piece? GetPiece(int y, int x);


        List<Piece?> GetAllPieces(PieceColor color);

        void Reset(Board board = null);

    }
}
