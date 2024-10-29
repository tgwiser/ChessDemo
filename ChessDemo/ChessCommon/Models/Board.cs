

namespace ChessCommon.Models;

public class Board
{
    public const int MAX_ROWS = 8;

    public const int MAX_COLS = 8;

    public bool whiteRightCastlingEnabled = true;
    public bool whiteLeftCastlingEnabled = true;
    public bool WhiteCastlingEnabled { get => whiteRightCastlingEnabled || whiteLeftCastlingEnabled; }

    public bool blackRightCastlingEnabled = true;
    public bool blackLeftCastlingEnabled = true;
    public bool BlackCastlingEnabled { get => blackRightCastlingEnabled || blackLeftCastlingEnabled; }

    public Board(Piece?[,] pieces)
    {
        Pieces = pieces;
    }

    public Piece?[,] Pieces { get; private set; }

    /// <summary>
    /// Returns Piece on given position
    /// </summary>
    /// <param name="pos">Position on chess board</param>
    public Piece? this[Position pos] => pos.HasValue ? Pieces[pos.Y, pos.X] : null;

    /// <summary>
    /// Returns Piece on given position
    /// </summary>
    /// <param name="x">0->8</param>
    /// <param name="y">0->8</param>
    public Piece? this[int x, int y] => Pieces[y, x];

    public (bool LeftCastlingEnabled, bool RightCastlingEnabled) GetCastleState(PieceColor color)
    {
        return (color == PieceColor.White) ?
            (whiteLeftCastlingEnabled,whiteRightCastlingEnabled) :
            (blackLeftCastlingEnabled,blackRightCastlingEnabled);
    }

    public void UpdateCastleState(PieceColor color, bool? isLeftCastlingEnabled,bool? isRightCastlingEnabled)
    {
        if (color == PieceColor.White)
        {
            if (isLeftCastlingEnabled.HasValue)
                whiteLeftCastlingEnabled = isLeftCastlingEnabled.Value;

            if (isRightCastlingEnabled.HasValue)
                whiteRightCastlingEnabled = isRightCastlingEnabled.Value;
        }
        else
        {
            if (isLeftCastlingEnabled.HasValue)
                blackLeftCastlingEnabled = isLeftCastlingEnabled.Value;

            if (isRightCastlingEnabled.HasValue)
                blackRightCastlingEnabled = isRightCastlingEnabled.Value;
        }
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
