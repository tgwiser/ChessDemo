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
        string[] moves;
        int idx = 0;

        public PgnAnalyzerService(IBoardManagerService boardManagerService, IPositionEvaluatorService positionEvaluatorService)
        {
            _boardManagerService = boardManagerService;
            _positionEvaluatorService = positionEvaluatorService;
        }

        public (Move MoveWhite, Move MoveBlack) Next()
        {
            Move moveWhite = null;
            Move moveBlack = null;
            try
            {
                string move = moves[idx];


                var whiteMoveStr = move.Split(" ")[0].TrimEnd('+');
                moveWhite = CreatePgnMove(whiteMoveStr, PieceColor.White);
                _boardManagerService.DropPiece(moveWhite);

                var blackMoveStr = move.Split(" ")[1].TrimEnd('+');
                 moveBlack = CreatePgnMove(blackMoveStr, PieceColor.Black);
                _boardManagerService.DropPiece(moveBlack);

                _boardManagerService.RestorePiece(moveBlack);
                _boardManagerService.RestorePiece(moveWhite);


                idx++;
                return (moveWhite, moveBlack);
            }
            catch (Exception ex)
            {
            }
            return (moveWhite, moveBlack);
        }

        public void GetGame(string input)
        {
            //string input = "1.e4 Nc6 2.d4 e5 3.d5 Nce7 4.Nf3 d6 5.Bd3 Ng6 6.Be3 Be7 7.Nc3 Nf4 8.Bb5+ Kf8 " +
            //   "9.Bf1 g5 10.Qd2 a6 11.g3 Bg4 12.Nxg5 Bxg5 13.gxf4 exf4 14.Bxf4 Bf6 15.Be2 Bxe2 " +
            //   "16.Qxe2 Bxc3+ 17.bxc3 Qf6 18.Qe3 Re8 19.O-O-O h6 20.Rhe1 Rh7 21.e5 Qf5 22.Qe4 Qxe4 " +
            //   "23.Rxe4 Nf6 24.Rc4 dxe5 25.Bg3 c6 26.dxc6 bxc6 27.Rxc6 Ne4 28.Rd7 h5 29.Bh4 Rg7 " +
            //   "30.f3 Ng5 31.Rf6 Ne6 32.Rh6 Rg1+ 33.Rd1 Rxd1+ 34.Kxd1 Kg7 35.Rf6 Rc8 36.Rf5 Rxc3 " +
            //   "37.Bf6+ Kg6 38.Bxe5 Rc5 39.Rf6+ Kg5 40.Bb2 Rb5 41.Bc3 Rc5 42.Ba1 Rc7 43.Kc1 Rd7 " +
            //   "44.Be5 Rd5 45.f4+ Nxf4 46.h4+ Kxh4 47.Rxf4+ Kg5 48.Bb8 f5 49.Rf2 h4 50.c4 Rd8 " +
            //   "51.Rg2+ Kh5 52.Bf4 h3 53.Rg5+ Kh4 54.Rxf5 Kg4 55.Rf7 Rd4 56.Bc7 Rxc4+ 57.Kb2 Rb4+ " +
            //   "58.Kc3 Rb7 59.Rf4+ Kg5 60.Rc4 Kf5 61.a4 Ke6 62.Bh2 Kd5 63.Rh4 Rf7 64.Rxh3 Kc6 " +
            //   "65.Kb4 Kb7 66.Rh5 Rg7 67.Ka5 Rd7 68.Bg1 Rg7 69.Bc5 Rc7 70.Re5 Rc6 71.Be3 Rc7 " +
            //   "72.Re6 Rf7 73.Rb6+ Ka8 74.Rxa6+ Kb8 75.Bc5 Rh7 76.Rg6 Kb7 77.Kb5 Rh8 78.Rg7+ Kb8 " +
            //   "79.Bd6+ Ka8 80.Ka6  1-0";

            // Regular expression to split by move numbers (e.g., "1.", "2.", etc.)
            moves = Regex.Split(input, @"\b\d+\.")
                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                   .Select(x => x.Trim())
                                   .ToArray();

    
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