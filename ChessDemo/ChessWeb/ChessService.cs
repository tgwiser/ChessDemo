using ChessCommon;
using ChessCommon.Evaluators.Contracts;
using ChessCommon.Models;
using ChessCommon.Persistense;
using System.Text;

namespace ChessWeb
{
    public class ChessService
    {
        IPositionEvaluator PositionEvaluator;
        IBoardManager BoardManager;
        IGameEvaluator GameEvaluator;
        IGamePersistenseManager _gamePersistenseManager;

        public ChessService(IPositionEvaluator positionCalculator,IBoardManager boardManager, IGameEvaluator gameEvaluator, IGamePersistenseManager gamePersistenseManager)
        {
            PositionEvaluator = positionCalculator;
            BoardManager = boardManager;
            _gamePersistenseManager = gamePersistenseManager;
        }
        public ChessEngine GetChessEngine()
        {
            ChessEngine ce = new ChessEngine(PositionEvaluator, BoardManager, GameEvaluator, _gamePersistenseManager);
            return ce;
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

        public static string LoadBoard(ChessEngine chessEngine, string fileName)
        {
            chessEngine.LoadBoard(fileName);
            return "file loaded";
        }


        public static (bool IsCheck, bool IsMate) GetCheckStatus(ChessEngine chessEngine, PieceColor color)
        {
            var isCheck = chessEngine.IsCheck(color);
            var isMate = chessEngine.IsMate(color);
            return (isCheck, isMate);
        }
    }






}
