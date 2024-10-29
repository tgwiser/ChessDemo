
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

    public (bool, bool) GetCastleState(PieceColor color)
    {
        return (color == PieceColor.White) ?
            (whiteRightCastlingEnabled, whiteLeftCastlingEnabled) :
            (blackRightCastlingEnabled, blackLeftCastlingEnabled);
    }

    public void UpdateCastleState(PieceColor color, bool newStatus, bool isLeftCastlingEnabled,bool isRightCastlingEnabled)
    {
        if (color == PieceColor.White)
        {
            if (isRightCastlingEnabled)
                whiteRightCastlingEnabled = newStatus;

            if (isLeftCastlingEnabled)
                whiteLeftCastlingEnabled = newStatus;
        }
        else
        {
            if (isRightCastlingEnabled)
                blackRightCastlingEnabled = newStatus;

            if (isLeftCastlingEnabled)
                blackLeftCastlingEnabled = newStatus;
        }
    }
}
