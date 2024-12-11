using ChessCommon;
using ChessCommon.Models;
using ChessCommon.Services.Contracts;
using System.Text;

namespace ChessWeb
{
    public class ChessService
    {
        IPositionEvaluatorService PositionEvaluator;
        IBoardManagerService BoardManager;
        IGameEvaluatorService _gameEvaluator;
        IGamePersistenseService _gamePersistenseManager;
        IChessEngine _chessEngine;
        IPgnAnalyzerService _pgnAnalyzerService;
        IGameHistoryService _gameHistoryService;

        public ChessService(
            IPositionEvaluatorService positionCalculator,
            IBoardManagerService boardManager,
            IGameEvaluatorService gameEvaluator,
            IGamePersistenseService gamePersistenseManager,
            IPgnAnalyzerService pgnAnalyzerService,
            IGameHistoryService gameHistoryService)
        {
            PositionEvaluator = positionCalculator;
            BoardManager = boardManager;
            _gamePersistenseManager = gamePersistenseManager;
            _gameEvaluator = gameEvaluator;
            _pgnAnalyzerService = pgnAnalyzerService;
            _gameHistoryService = gameHistoryService;
            _chessEngine = new ChessEngine(PositionEvaluator, BoardManager, _gameEvaluator, _gamePersistenseManager, _pgnAnalyzerService, _gameHistoryService);
        }


        public string PlayBestMove(Piece? piece)
        {
            if (piece == null)
                return null;

            string value = "♔";
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            int pieceSymbolValue = GetSymbolValue(piece);

            bytes[2] += (byte)pieceSymbolValue;
            var w221 = Encoding.UTF8.GetString(bytes);
            return w221;
        }


        public static string GetIcon(Piece? piece)
        {
            if (piece == null)
                return null;

            string value = "♔";
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            int pieceSymbolValue = GetSymbolValue(piece);

            bytes[2] += (byte)pieceSymbolValue;
            var w221 = Encoding.UTF8.GetString(bytes);
            return w221;
        }

        public static int GetSymbolValue(Piece? piece)
        {
            int pieceSymbolValue = piece.Color == PieceColor.Black ? 6 : 0;
            return piece.Type switch
            {
                PieceType.King => pieceSymbolValue,
                PieceType.Queen => pieceSymbolValue + 1,
                PieceType.Rook => pieceSymbolValue + 2,
                PieceType.Bishop => pieceSymbolValue + 3,
                PieceType.Knight => pieceSymbolValue + 4,
                PieceType.Pawn => pieceSymbolValue + 5,
                _ => throw new Exception("Not existing piece"),
            };
        }

    }

}
