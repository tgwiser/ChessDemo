using ChessCommon.Models;

namespace ChessCommon.Services.Contracts;

public interface IGameEvaluator
{
    int BestValue { get; }
    int Counter { get; }
    Move? SelectedMove { get; }

    Move? EvaluateBestMove(int depth, PieceColor selectedPlayer);
    void InitPlayersPieces();
}
