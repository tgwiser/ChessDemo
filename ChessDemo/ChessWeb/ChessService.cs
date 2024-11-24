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
        IGameEvaluator _gameEvaluator;
        IGamePersistenseManager _gamePersistenseManager;

        public ChessService(IPositionEvaluator positionCalculator,IBoardManager boardManager, IGameEvaluator gameEvaluator, IGamePersistenseManager gamePersistenseManager)
        {
            PositionEvaluator = positionCalculator;
            BoardManager = boardManager;
            _gamePersistenseManager = gamePersistenseManager;
            _gameEvaluator = gameEvaluator;

        }
        
        public ChessEngine GetChessEngine()
        {
            ChessEngine ce = new ChessEngine(PositionEvaluator, BoardManager, _gameEvaluator, _gamePersistenseManager);
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

        public async static Task<List<string>> FindGames(ChessEngine chessEngine)
        {
            return await chessEngine.FindGames();
        }

        public async static Task<List<string>> FindGames(ChessEngine chessEngine, string filter)
        {
            return await chessEngine.FindGames(filter);
        }

        internal static async Task<List<string>> DeleteGame(ChessEngine chessEngine, string name)
        {
            await chessEngine.DeleteGame(name);
            return await chessEngine.FindGames();
        }

        internal static async Task LoadGame(ChessEngine chessEngine, string name)
        {
            await chessEngine.LoadBoard(name);
        }
    }






}
