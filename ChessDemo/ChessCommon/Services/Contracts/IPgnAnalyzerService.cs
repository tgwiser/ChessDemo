using ChessCommon.Models;

namespace ChessCommon.Services.Contracts
{
    public interface IPgnAnalyzerService
    {
        List<(Move WhiteMove, Move BlackMove)> LoadGame(string input);

    }
}
