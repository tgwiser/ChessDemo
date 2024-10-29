

namespace ChessCommon.Models;

public class Board
{
    public const int MAX_ROWS = 8;

    public const int MAX_COLS = 8;

    public bool whiteRightCastlingEnabled = true;
    public bool whiteLeftCastlingEnabled = true;

    public bool blackRightCastlingEnabled = true;
    public bool blackLeftCastlingEnabled = true;
 
    public Board(Piece?[,] pieces)
    {
        Pieces = pieces;
    }

    public Piece?[,] Pieces { get; private set; }

    
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
            whiteLeftCastlingEnabled = v;
        else
            blackLeftCastlingEnabled = v;
    }

    internal void UpdateRightCastleState(PieceColor color, bool v)
    {
        if (color == PieceColor.White)
            whiteRightCastlingEnabled = v;
        else
            blackRightCastlingEnabled = v;
    }
}
