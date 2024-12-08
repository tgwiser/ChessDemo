using ChessCommon.Models;

namespace ChessCommon
{
    public interface IChessEngine
    {
        PieceColor CurrentPlayer { get; }

        Task DeleteGame(string name);
        void DropPiece(Position originalPosition, Position destination);
        (string SelectedMove, int Counter, int BestValue) EvaluateBestMove(int depth, PieceColor color);
        Task<List<string>> FindGames();
        bool IsLegalMove(Position originalPosition, Position destination);
        Task LoadBoard(string fileName);
        void Next();
        void Prev();
        Task<List<string>> FindGames(string filter);
        void SaveBoard(string fileName);
        void PlayBestMove(int depth);
        Piece GetPiece(Position position);
        List<Move> GetMoves();

        (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) WhiteCastlingState { get; }

        (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) BlackCastlingState { get; }


        void LoadPgnBoard(string fileName);

        void ResetPgnMoves();
    }
}