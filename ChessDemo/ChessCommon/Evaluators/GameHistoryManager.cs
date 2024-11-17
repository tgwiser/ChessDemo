using ChessCommon.Models;

namespace ChessCommon.Evaluators;

internal class GameHistoryManager
{
    List<Move> gameHistory = new List<Move>();
    int historyIndex = 0;

    public GameHistoryManager() { }

    public void AddMove(Move move)
    {
        if (gameHistory.Count == historyIndex)
        {
            gameHistory.Add(move);
            historyIndex = gameHistory.Count;
        }
    }

    public bool TryGetNextMove(out Move? move)
    {
        move = null;
        if (historyIndex < gameHistory.Count)
        {
            move = gameHistory[historyIndex];
            historyIndex++;
        }
        return move != null;
    }
    public bool TryGetPrevMove(out Move? move)
    {
        move = null;
        if (historyIndex > 0)
        {
            historyIndex--;
            move = gameHistory[historyIndex];
        }
        return move!=null;
    }
}
