
namespace ChessCommon.Models;


public class Move
{
    public bool IsCastle;

    public Castle? Castle;

    public bool IsLeftRock;
    public bool IsRightRock;

    /// <summary>
    /// Initializes new Move object by given positions
    /// </summary>
    public Move(Position originalPosition, Position newPosition, Piece piece, Piece? capturedPiece)
    {
        SrcPosition = originalPosition;
        DestPosition = newPosition;

        Piece = piece;
        CapturedPiece = capturedPiece;

        IsCastle = piece.Type == PieceType.King && Math.Abs(originalPosition.X - newPosition.X) >= 2;
        IsLeftRock = piece.Type == PieceType.Rook && originalPosition.X == 0;
        IsRightRock = piece.Type == PieceType.Rook && originalPosition.X == 7;
    }


    /// <summary>
    /// Moved Piece
    /// </summary>
    public Piece Piece { get; set; }

    /// <summary>
    /// Original position of moved piece
    /// </summary>
    public Position SrcPosition { get; set; }

    /// <summary>
    /// New Position of moved piece
    /// </summary>
    public Position DestPosition { get; set; }

    /// <summary>
    /// Captured piece (if exist) or null
    /// </summary>
    public Piece? CapturedPiece { get; set; }

    /// <summary>
    /// Move places opponent's king in check? => true
    /// </summary>
    public bool IsCheck { get; internal set; }

    /// <summary>
    /// Move places opponent's king in checkmate => true
    /// </summary>
    public bool IsMate { get; internal set; }

    /// <summary>
    /// Whether Positions are initialized
    /// </summary>
    /// 
    public bool HasValue => SrcPosition.HasValue && DestPosition.HasValue;


    public bool? LeftCastlingEnabled { get; internal set; }
    public bool? RightCastlingEnabled { get; internal set; }

    public override string ToString()
    {
        return $"{SrcPosition} + ->  + {DestPosition}";
    }

}
