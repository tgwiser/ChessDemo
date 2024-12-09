using ChessCommon.Models;

namespace ChessCommon.Services.Contracts
{
    public interface IPgnAnalyzerService
    {
        //List<(Move WhiteMove, Move BlackMove)> GetMovesFromPgnSrc(string input);
        List<string> GetPgnMovesFromPgnSrc(string input);


        //  List<(string WhiteMove, string BlackMove)> ConvertToPgnMoves(List<Move> moves);
        public Move GetMoveFromPgn(string moveStr, PieceColor pieceColor);

        public string ConvertToPgnMove(Move move, PieceColor pieceColor);
    }
}
