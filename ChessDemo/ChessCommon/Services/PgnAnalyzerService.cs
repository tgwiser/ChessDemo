using ChessCommon.Common;
using ChessCommon.Extensions;
using ChessCommon.Models;
using ChessCommon.Services.Contracts;
using System.Text.RegularExpressions;

namespace ChessCommon.Services;

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

        List<string> pgnMoves = [];

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
        Position pieceDest = GetDestFromPgn(moveStr, pieceColor);

        var destPiece = _boardManagerService.GetPiece(pieceDest);
        var srcPiece = GetSrcFromPgn(moveStr, pieceDest, pieceColor);

        Move move = new Move(srcPiece.Position, pieceDest, srcPiece, destPiece);
        return move;

    }

    private Piece GetSrcFromPgn(string move, Position destPosition, PieceColor pieceColor)
    {
        MoveType moveType = GetMoveType(move);

        switch (moveType)
        {
            case MoveType.SmallCastle:
            case MoveType.LargeCastle:
                return GetSrcFromPgnCastle(pieceColor);
            case MoveType.Pawn:
                return GetSrcFromPgnPawn(move, pieceColor);
            case MoveType.Piece:
                return GetSrcFromPgnPiece(move, destPosition, pieceColor);
        }

        throw new NotImplementedException("Cannot parse move");
    }

    private Piece GetSrcFromPgnPiece(string pgnMove, Position destPosition, PieceColor pieceColor)
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
                return piece;
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
                    return piece;
            }
        }

        throw new NotImplementedException("Cannot parse move");

    }

    private Piece GetSrcFromPgnPawn(string move, PieceColor pieceColor)
    {
        Piece piece;
        int factor = pieceColor == PieceColor.White ? -1 : 1;

        int srcXPosition = CommonUtils.PositionFromFile(move[0]);
        short destYPosition = CommonUtils.PositionFromRank(move[move.Length - 1]);

        bool isCapture = move.Contains("x");
        if (isCapture)
        {
            piece = _boardManagerService.GetPiece(destYPosition + factor, srcXPosition)!;
            return piece;
        }
        else
        {
            piece = _boardManagerService.GetPiece(destYPosition + factor, srcXPosition)!;
            if (piece != null)
                return piece;
            piece = _boardManagerService.GetPiece(destYPosition + factor * 2, srcXPosition)!;
            if (piece != null)
                return piece;
        }

        throw new NotImplementedException("Cannor parse move");
    }


    //From Move  to Pgn
    public string ConvertToPgnMove(Move move, PieceColor pieceColor)
    {
        string moveStr = move.ToString();
        string captureChar = move.CapturedPiece == null ? string.Empty : "x";


        PieceType pieceType = move.Piece.Type;

        if (move.IsCastle)
            return move.Castle!.IsRightCastle ? "O-O" : "O-O-O";

        if (TryGetPawneMove(move, ref moveStr))
            return moveStr;

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

    public Piece GetSrcFromPgnCastle(PieceColor color)
    {
        var piece = _boardManagerService.GetPiece(color == PieceColor.White ? 0 : 7, 4)!;
        return piece;
    }

    public static MoveType GetMoveType(string pgnMove)
    {
        if (pgnMove == "O-O-O")
            return MoveType.LargeCastle;

        if (pgnMove == "O-O")
            return MoveType.SmallCastle;

        return (CommonUtils.GetPgnPieceType(pgnMove) == PieceType.Pawn) ?
            MoveType.Pawn :
            MoveType.Piece;

    }

    public static bool TryGetPawneMove(Move move, ref string pgnMove)
    {
        if (move.Piece.Type == PieceType.Pawn)
        {
            string captureChar = move.CapturedPiece == null ? string.Empty : "x";

            pgnMove = move.CapturedPiece == null ?
                $"{move.DestPosition}" :
                $"{CommonUtils.GetPositionFile(move.SrcPosition)}{captureChar}{move.DestPosition}";
        }

        return move.Piece.Type == PieceType.Pawn;
    }

    public static bool TryGetDestFromPgnCastle(string move, PieceColor color, out Position position)
    {
        position = null;
        var moveType = GetMoveType(move);

        if (moveType == MoveType.LargeCastle)
            position = new Position(color == PieceColor.White ? 0 : 7, 2);

        if (moveType == MoveType.SmallCastle)
            position = new Position(color == PieceColor.White ? 0 : 7, 6);

        return position != null;

    }

    public static Position GetDestFromPgn(string move, PieceColor color)
    {
        //CheckFor castle
        if (TryGetDestFromPgnCastle(move, color, out var position))
            return position;

        string destString = move.Substring(move.Length - 2);


        int yDest = CommonUtils.PositionFromFile(destString[1]);
        int xDest = CommonUtils.PositionFromRank(destString[0]);

        return new Position(yDest, xDest);
    }
}