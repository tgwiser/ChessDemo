using ChessCommon;
using ChessCommon.Services.Contracts;
using ChessCommon.Models;
using System.Text;
using System.Runtime.CompilerServices;

namespace ChessWeb
{
    public class ChessService
    {
        IPositionEvaluator PositionEvaluator;
        IBoardManager BoardManager;
        IGameEvaluator _gameEvaluator;
        IGamePersistenseService _gamePersistenseManager;
        IChessEngine _chessEngine;

        public ChessService(IPositionEvaluator positionCalculator,IBoardManager boardManager, IGameEvaluator gameEvaluator, IGamePersistenseService gamePersistenseManager)
        {
            PositionEvaluator = positionCalculator;
            BoardManager = boardManager;
            _gamePersistenseManager = gamePersistenseManager;
            _gameEvaluator = gameEvaluator;
            _chessEngine = new ChessEngine(PositionEvaluator, BoardManager, _gameEvaluator, _gamePersistenseManager);
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
