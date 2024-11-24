using ChessCommon.Models;
using System.Text;

namespace ChessCommon.Services;

internal class GameHistoryService
{
    List<Move> gameHistory = new List<Move>();
    int historyIndex = 0;

    public GameHistoryService() { }

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

    internal List<Move> GetMoves()
    {
        return gameHistory;
    }
}
