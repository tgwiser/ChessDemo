using ChessCommon.Services;
using ChessCommon.Services.Contracts;

namespace ChessCommon.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            IBoardManagerService b = new BoardManagerService();
            b.Reset();
            IPositionEvaluatorService p = new PositionEvaluatorService(b);
            IPgnAnalyzerService pgn = new PgnAnalyzerService(b, p);
            //  pgn.GetMovesFromPgnSrc("");
        }
    }
}