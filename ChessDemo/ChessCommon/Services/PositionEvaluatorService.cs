using ChessCommon.Services.Contracts;
using ChessCommon.Models;

namespace ChessCommon.Services;

public class PositionEvaluatorService : IPositionEvaluatorService
{
    private IBoardManagerService _boardManager;

    public PositionEvaluatorService( IBoardManagerService boardManager)
    {
        _boardManager = boardManager;
    }

    public List<Position> GetLegalPositions(Piece piece)
    {
        var legalPositions = new List<Position>();
        Position position = piece.Position;
        switch (piece.Type)
        {
            case PieceType.Pawn:
                legalPositions = GetLegalPawnPositions(position, piece);
                break;
            case PieceType.King:
                legalPositions = GetLegalKingPositions(piece, piece.Color);
                break;
            case PieceType.Rook:
                legalPositions = GetLegalRockPositions(piece, piece.Color);
                break;
            case PieceType.Bishop:
                legalPositions = GetLegalBishopPositions(piece, piece.Color);
                break;
            case PieceType.Knight:
                legalPositions = GetLegalNightPositions(piece, piece.Color);
                break;
            case PieceType.Queen:
                legalPositions = GetLegalQueenPositions(piece, piece.Color);
                break;
        }

        return legalPositions;
    }

    private List<Position> GetLegalPawnPositions(Position position, Piece piece)
    {
        short direction = (short)(piece.Color == PieceColor.White ? 1 : -1);
        List<Position> legalMoves = new();

        //Move up
        AddMovePawnPosition(piece, direction, position.X, position.Y, ref legalMoves);

        //Eat
        AddEatPawnPosition(piece, position.X - 1, position.Y + direction, ref legalMoves);
        AddEatPawnPosition(piece, position.X + 1, position.Y + direction, ref legalMoves);

        return legalMoves;
    }

    private void AddMovePawnPosition(Piece piece, short direction, int x, int y, ref List<Position> legalMoves)
    {
        var newPosition = new Position(y + direction, x);

        if (newPosition.HasValue)
        {
            var destPiece = _boardManager.GetPiece(newPosition);

            if (destPiece == null)
            {
                legalMoves.Add(newPosition);

                int pawnInitPosition = piece.Color == PieceColor.White ? 1 : 6;
                if (y == pawnInitPosition)
                    AddMovePawnPosition(piece, direction, x, newPosition.Y, ref legalMoves);
            }
        }
    }

    private void AddEatPawnPosition(Piece piece, int x, int y, ref List<Position> legalMoves)
    {
        var newPosition = new Position(y, x);
        if (newPosition.HasValue)
        {
            var destPiece = _boardManager.GetPiece(newPosition);
            if (destPiece != null && piece.Color != destPiece.Color)
                legalMoves.Add(newPosition);
        }
    }

    private List<Position> GetLegalKingPositions(Piece piece, PieceColor pieceColor)
    {
        List<Position> legalMoves = [];

        (bool leftCastlingEnabled,bool rightCastlingEnabled) = _boardManager.GetCastleState(pieceColor);

        for (short x = -1; x <= 1; x++)
        {
            for (short y = -1; y <= 1; y++)
            {
                if (x != 0 || y != 0)
                {
                    Position newPosition = new Position(piece.Position.Y + y, piece.Position.X + x);
                    TryAddPosition(pieceColor, newPosition, ref legalMoves);
                }
            }
        }

        if (rightCastlingEnabled)
            TryAppendSmallCastlePosition(piece.Position, ref legalMoves);

        if (leftCastlingEnabled)
            TryAppendLargeCastlePosition(piece.Position, ref legalMoves);

        return legalMoves;
    }

    private List<Position> GetLegalRockPositions(Piece piece, PieceColor pieceColor)
    {
        List<Position> legalMoves = [];
        var position = piece.Position;

        //right
        for (short x = 1; x < CommonUtils.MAX_ROWS; x++)
        {
            if (!TryAddPosition(pieceColor, position.X + x, position.Y, ref legalMoves))
                break;
        }

        //left
        for (short x = -1; x > -CommonUtils.MAX_ROWS; x--)
        {
            if (!TryAddPosition(pieceColor, position.X + x, position.Y, ref legalMoves))
                break;
        }

        //up
        for (short y = 1; y < CommonUtils.MAX_ROWS; y++)
        {
            if (!TryAddPosition(pieceColor, position.X, position.Y + y, ref legalMoves))
                break;
        }

        //down
        for (short y = -1; y > -CommonUtils.MAX_ROWS; y--)
        {
            if (!TryAddPosition(pieceColor, position.X, position.Y + y, ref legalMoves))
                break;
        }

        return legalMoves;
    }

    private List<Position> GetLegalBishopPositions(Piece piece, PieceColor pieceColor)
    {
        List<Position> legalMoves = new List<Position>();
        var position = piece.Position;
        //right-up
        int idx = 1;
        while (TryAddPosition(pieceColor, position.X + idx, position.Y + idx, ref legalMoves))
            idx++;

        //right-down
        idx = 1;
        while (TryAddPosition(pieceColor, position.X + idx, position.Y - idx, ref legalMoves))
            idx++;


        //left-up
        for (short x = -1, y = 1; x > -8; x--, y++)
        {
            if (!TryAddPosition(pieceColor, position.X + x, position.Y + y, ref legalMoves))
                break;
        }

        //left-down
        for (short y = -1; y > -8; y--)
        {
            if (!TryAddPosition(pieceColor, position.X + y, position.Y + y, ref legalMoves))
                break;
        }

        return legalMoves;
    }

    private List<Position> GetLegalNightPositions(Piece piece, PieceColor pieceColor)
    {
        List<Position> legalMoves = new();
        var position = piece.Position;
        TryAddPosition(pieceColor, position.X + 1, position.Y + 2, ref legalMoves);
        TryAddPosition(pieceColor, position.X + 1, position.Y - 2, ref legalMoves);
        TryAddPosition(pieceColor, position.X - 1, position.Y + 2, ref legalMoves);
        TryAddPosition(pieceColor, position.X - 1, position.Y - 2, ref legalMoves);
        TryAddPosition(pieceColor, position.X + 2, position.Y + 1, ref legalMoves);
        TryAddPosition(pieceColor, position.X + 2, position.Y - 1, ref legalMoves);
        TryAddPosition(pieceColor, position.X - 2, position.Y + 1, ref legalMoves);
        TryAddPosition(pieceColor, position.X - 2, position.Y - 1, ref legalMoves);
        return legalMoves;
    }

    private List<Position> GetLegalQueenPositions(Piece piece, PieceColor pieceColor)
    {
        var legalMoves = GetLegalRockPositions(piece, pieceColor);
        var legalBishopMoves = GetLegalBishopPositions(piece, pieceColor);

        legalMoves.AddRange(legalBishopMoves);
        return legalMoves;
    }

    private bool TryAddPosition(PieceColor pieceColor, int x, int y, ref List<Position> legalMoves)
    {
        Position newPosition = new Position(y, x);
        return TryAddPosition(pieceColor, newPosition, ref legalMoves);
    }

    private bool TryAddPosition(PieceColor pieceColor, Position newPosition, ref List<Position> legalMoves)
    {
        if (!newPosition.HasValue)
            return false;

        var destPiece = _boardManager.GetPiece(newPosition);

        if (destPiece == null || destPiece.Color != pieceColor)
        {
            legalMoves.Add(newPosition);
        }
        return destPiece == null;
    }

    private bool TryAppendSmallCastlePosition(Position position, ref List<Position> legalMoves)
    {
        for (int i = 1; i <= 2; i++)
        {
            var kingPosition = new Position(position.Y, position.X + i);
            if (_boardManager.GetPiece(kingPosition) != null)
                return false;

            if (i == 2)
                legalMoves.Add(kingPosition);
        }
        return true;
    }

    private bool TryAppendLargeCastlePosition(Position position, ref List<Position> legalMoves)
    {
        var kingPosition = new Position(position.Y, position.X - 2);
        for (int i = 1; i <= 3; i++)
        {
            if (_boardManager.GetPiece(new Position(position.Y, position.X - i)) != null)
                return false;

            if (i == 3)
                legalMoves.Add(kingPosition);
        }
        return true;
    }
}
