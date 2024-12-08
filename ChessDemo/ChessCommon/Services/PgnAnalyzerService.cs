using ChessCommon.Models;
using ChessCommon.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessCommon.Services
{
    public class PgnAnalyzerService : IPgnAnalyzerService
    {
        IBoardManagerService _boardManagerService;
        IPositionEvaluatorService _positionEvaluatorService;
       
        int idx = 0;

        public PgnAnalyzerService(IBoardManagerService boardManagerService, IPositionEvaluatorService positionEvaluatorService)
        {
            _boardManagerService = boardManagerService;
            _positionEvaluatorService = positionEvaluatorService;
        }


        public List<(Move WhiteMove, Move BlackMove)> LoadGame(string input)
        {
            _boardManagerService.Reset();

            // Regular expression to split by move numbers (e.g., "1.", "2.", etc.)
            string[] moves = Regex.Split(input, @"\b\d+\.")
                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                   .Select(x => x.Trim())
                                   .ToArray();

            List<(Move MoveWhite, Move MoveBlack)> pgnMoves = [];
          
            foreach (var move in moves)
            {
                var whiteMoveStr = move.Split(" ")[0].TrimEnd('+');
                var moveWhite = CreatePgnMove(whiteMoveStr, PieceColor.White);
                _boardManagerService.DropPiece(moveWhite);

                var blackMoveStr = move.Split(" ")[1].TrimEnd('+');
                var moveBlack = CreatePgnMove(blackMoveStr, PieceColor.Black);
                _boardManagerService.DropPiece(moveBlack);
                pgnMoves.Add((moveWhite, moveBlack));
            }

            _boardManagerService.Reset();

            return pgnMoves;

        }


     


        /// <summary>
        /// Initializes new Move object by given positions
        /// </summary>
        public Move CreatePgnMove(string moveStr, PieceColor pieceColor)
        {
            Position pieceDest = GetPieceDest(moveStr, pieceColor);
            Position pieceSrc = GetPieceSrc(moveStr, pieceColor);

            var destPiece = _boardManagerService.GetPiece(pieceDest);
            var srcPiece = _boardManagerService.GetPiece(pieceSrc);

            Move move = new Move(pieceSrc, pieceDest, srcPiece, destPiece);
            return move;
        }

        private static Position GetPieceDest(string move, PieceColor pieceColor)
        {
            //CheckFor castle
            int y = pieceColor == PieceColor.White ? 0 : 7;
            if (move == "O-O-O")
                return new Position(y, 2);

            if (move == "O-O")
                return new Position(y, 6);


            string destString = move.Substring(move.Length - 2);
            return new Position(destString);
        }

        private Position GetPieceSrc(string move, PieceColor pieceColor)
        {
            if (move == "O-O-O" || move == "O-O")
            {
                int y = pieceColor == PieceColor.White ? 0 : 7;
                return new Position(y, 4);
            }

            PieceType pieceType = GetPieceType(move);

            Position position = pieceType == PieceType.Pawn ? 
                GetPawnPieceSrc(move, pieceColor):
                GetNonPawnPieceSrc(move, pieceColor);
              
            return position;
        }



        private static PieceType GetPieceType(string move)
        {
            char pieceLetter = move[0];
            return pieceLetter switch
            {
                'N' => PieceType.Knight,
                'B' => PieceType.Bishop,
                'R' => PieceType.Rook,
                'Q' => PieceType.Queen,
                'K' => PieceType.King,
                _=> PieceType.Pawn
            };
        }

        private Position GetPawnPieceSrc(string move, PieceColor pieceColor)
        {
            Position position;
            int factor = pieceColor == PieceColor.White ? -1 : 1;

            short srcXPosition = CommonUtils.PositionFromFile(move[0]);
            short destYPosition = CommonUtils.PositionFromRank(move[move.Length - 1]);

            bool isCapture = move.Contains("x");
            if (isCapture)
            {
                position = new Position(destYPosition + factor,srcXPosition);
                return position;
            }
            else
            {
                position = new Position(destYPosition + factor,srcXPosition);
                if(_boardManagerService.GetPiece(position)!=null)
                    return position;

                 position = new Position(destYPosition + factor * 2,srcXPosition);
                if (_boardManagerService.GetPiece(position) != null)
                    return position;
            }


            throw new NotImplementedException("Cannor parse move");
        }

        private Position GetNonPawnPieceSrc(string move, PieceColor pieceColor)
        {
            PieceType pieceType = GetPieceType(move);
            int srcEndIdx = move.Contains("x") ? move.Length - 3 : move.Length - 2;
            string srcStr = move.Substring(1, srcEndIdx - 1);
            var pieces = _boardManagerService.GetAllPieces(pieceColor).Where(p => p.Type == pieceType);
            var destPosition = GetPieceDest(move, pieceColor);

            if (pieces.Count() == 0)
            {
                throw new NotImplementedException("Cannot parse move");
            }

            if (pieces.Count() == 1)
            {
                if (!string.IsNullOrEmpty(srcStr))
                    throw new NotImplementedException("Cannot parse move");

                var piece = pieces.FirstOrDefault()!;
                var legalPositions = _positionEvaluatorService.GetLegalPositions(piece);
                if (legalPositions.Exists(lp => lp == destPosition))
                    return piece.Position;
            }

            if (pieces.Count() > 1)
            {
                if (!string.IsNullOrEmpty(srcStr))
                {
                    if (short.TryParse(srcStr, out short legalYPosition))
                        pieces = pieces.Where(p => p.Position.Y == legalYPosition);
                    else
                        pieces = pieces.Where(p => p.Position.X == CommonUtils.PositionFromFile(srcStr[0]));
                }

                foreach (var piece in pieces)
                {
                    var legalPositions = _positionEvaluatorService.GetLegalPositions(piece);
                    if (legalPositions.Exists(lp => lp == destPosition))
                        return piece.Position;
                }

            }

            throw new NotImplementedException("Cannot parse move");

        }
    }
}