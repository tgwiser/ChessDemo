

namespace ChessCommon.Models;

public class Board
{
    public const int MAX_ROWS = 8;
    public const int MAX_COLS = 8;

    public bool IsWhiteLeftCastlingEnabled { get; private set; } = true;

    public bool IsWhiteRightCastlingEnabled { get; private set; } = true;
    public bool IsBlackLeftCastlingEnabled { get; private set; } = true;
    public bool IsBlackRightCastlingEnabled { get; private set; } = true;

    public Piece?[,] Pieces { get; private set; }

    public Board(Piece?[,] pieces)
    {
        Pieces = pieces;
    }

    public void InitState(bool isWhiteLeftCastlingEnabled, bool isWhiteRightCastlingEnabled, bool isBlackLeftCastlingEnabled, bool isBlackRightCastlingEnabled)
    {
        IsWhiteLeftCastlingEnabled = isWhiteLeftCastlingEnabled;
        IsWhiteRightCastlingEnabled = isWhiteRightCastlingEnabled;
        IsBlackLeftCastlingEnabled = isBlackLeftCastlingEnabled;
        IsBlackRightCastlingEnabled = isBlackRightCastlingEnabled;
    }


    /// <summary>
    /// Returns Piece on given position
    /// </summary>
    /// <param name="x">0->8</param>
    /// <param name="y">0->8</param>
    public Piece? this[int x, int y] => Pieces[y, x];

    public Piece? this[Position pos]
    {
        get => pos.HasValue ? Pieces[pos.Y, pos.X] : null;
        set => Pieces[pos.Y, pos.X] = value;
    }

    internal void UpdateLeftCastleState(PieceColor color, bool v)
    {
        if (color == PieceColor.White)
            IsWhiteLeftCastlingEnabled = v;
        else
            IsBlackLeftCastlingEnabled = v;
    }

    internal void UpdateRightCastleState(PieceColor color, bool v)
    {
        if (color == PieceColor.White)
            IsWhiteRightCastlingEnabled = v;
        else
            IsBlackRightCastlingEnabled = v;
    }
}
