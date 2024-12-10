using ChessCommon.Common;
using ChessCommon.Extensions;
using ChessCommon.Models;
using ChessCommon.Services.Contracts;
using System.Text.RegularExpressions;

namespace ChessCommon.Services
{
    public class PgnAnalyzerService : IPgnAnalyzerService
    {
        IBoardManagerService _boardManagerService;
        IPositionEvaluatorService _positionEvaluatorService;

        public PgnAnalyzerService(IBoardManagerService boardManagerService, IPositionEvaluatorService positionEvaluatorService)
        {
            _boardManagerService = boardManagerService;
            _positionEvaluatorService = positionEvaluatorService;
        }


        //From Pgn to Move

        public List<string> GetPgnMovesFromPgnSrc(string pgnString)
        {
            // Regular expression to split by move numbers (e.g., "1.", "2.", etc.)
            string[] moves = Regex.Split(pgnString, @"\b\d+\.")
                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                   .Select(x => x.Trim())
                                   .ToArray();

            List<string >pgnMoves = [];

            foreach (var move in moves)
            {
                var moveArr = move.Split(" ");
                var whiteMoveStr = moveArr[0].TrimEnd('+');
                pgnMoves.Add(whiteMoveStr);

                if (moveArr.Length > 1)
                {
                    var blackMoveStr = moveArr[1].TrimEnd('+');
                    pgnMoves.Add(blackMoveStr);
                }
            }

            return pgnMoves;

        }

        public Move GetMoveFromPgn(string moveStr, PieceColor pieceColor)
        {
            try
            {
                Position pieceDest = GetDestFromPgn(moveStr, pieceColor);
                Position pieceSrc = GetSrcFromPgn(moveStr, pieceDest, pieceColor);

                var destPiece = _boardManagerService.GetPiece(pieceDest);
                var srcPiece = _boardManagerService.GetPiece(pieceSrc);

                Move move = new Move(pieceSrc, pieceDest, srcPiece, destPiece);
                return move;
            }
            catch (Exception ex) 
            {
                Position pieceDest = GetDestFromPgn(moveStr, pieceColor);
                Position pieceSrc = GetSrcFromPgn(moveStr, pieceDest, pieceColor);

                var destPiece = _boardManagerService.GetPiece(pieceDest);
                var srcPiece = _boardManagerService.GetPiece(pieceSrc);

                Move move = new Move(pieceSrc, pieceDest, srcPiece, destPiece);
                return move;
            }
        }

        private static Position GetDestFromPgn(string move, PieceColor pieceColor)
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

        private Position GetSrcFromPgn(string move, Position destPosition, PieceColor pieceColor)
        {
            MoveType moveType = CommonUtils.GetMoveType(move);

            switch (moveType)
            {
                case MoveType.Castle:
                    return GetSrcFromPgnCastle(pieceColor);
                case MoveType.Pawn:
                    return GetSrcFromPgnPawn(move, pieceColor);
                case MoveType.Piece:
                    return GetSrcFromPgnPiece(move, destPosition, pieceColor);
            }

            throw new NotImplementedException("Cannot parse move");
        }

        private Position GetSrcFromPgnPiece(string pgnMove, Position destPosition, PieceColor pieceColor)
        {

            PieceType pieceType = CommonUtils.GetPgnPieceType(pgnMove);
            int srcEndIdx = pgnMove.Contains("x") ? pgnMove.Length - 3 : pgnMove.Length - 2;
            string srcStr = pgnMove.Substring(1, srcEndIdx - 1);
            var pieces = _boardManagerService.GetAllPieces(pieceColor).Where(p => p.Type == pieceType);
         
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

        private Position GetSrcFromPgnPawn(string move, PieceColor pieceColor)
        {
            Position position;
            int factor = pieceColor == PieceColor.White ? -1 : 1;

            short srcXPosition = CommonUtils.PositionFromFile(move[0]);
            short destYPosition = CommonUtils.PositionFromRank(move[move.Length - 1]);

            bool isCapture = move.Contains("x");
            if (isCapture)
            {
                position = new Position(destYPosition + factor, srcXPosition);
                return position;
            }
            else
            {
                position = new Position(destYPosition + factor, srcXPosition);
                if (_boardManagerService.GetPiece(position) != null)
                    return position;

                position = new Position(destYPosition + factor * 2, srcXPosition);
                if (_boardManagerService.GetPiece(position) != null)
                    return position;
            }


            throw new NotImplementedException("Cannor parse move");
        }

        private Position GetSrcFromPgnCastle(PieceColor pieceColor)
        {
            int y = pieceColor == PieceColor.White ? 0 : 7;
            return new Position(y, 4);
        }

        //From Move  to Pgn

        public string ConvertToPgnMove(Move move, PieceColor pieceColor)
        {
            string moveStr = move.ToString();
            string captureChar = move.CapturedPiece == null ? string.Empty : "x";
          

            PieceType pieceType = move.Piece.Type;

            if (move.IsCastle)
            {
                moveStr = move.Castle!.IsRightCastle ? "O-O" : "O-O-O";
                return moveStr;
            }

            if (pieceType == PieceType.Pawn)
            {
                moveStr = move.CapturedPiece == null ? 
                    $"{move.DestPosition}" : 
                    $"{CommonUtils.GetPositionFile(move.SrcPosition)}{captureChar}{move.DestPosition}";

                return moveStr;
            }

            var pieces = _boardManagerService
                .GetAllPieces(pieceColor)
                .Where(p => p!.Type == pieceType)
                .Where(p => _positionEvaluatorService.GetLegalPositions(p!).Any(p => p == move.DestPosition));

            var srcIndication = string.Empty;
            if (pieces.Select(p => p.Position.X).Distinct().Count() > 1)
                srcIndication = CommonUtils.GetPositionFile(move.Piece.Position);

            if (pieces.Select(p => p.Position.Y).Distinct().Count() > 1)
                srcIndication = move.Piece.Position.Y.ToString();

            moveStr = $"{pieceType.ToPgn()}{srcIndication}{captureChar}{move.DestPosition}";
            return moveStr;
        }


   

    }


}