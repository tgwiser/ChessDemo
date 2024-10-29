
namespace ChessCommon.Models;

public class Piece
{
    public PieceType? OriginalPieceType { get; set; }

    public PieceColor Color { get; }

    public PieceType Type { get; set; }

    public Position Position { get; set; }

    public Piece(PieceColor color, PieceType type, Position position)
    {
        Color = color;
        Type = type;
        Position = position;
    }

    public override string ToString() => $"{Color},{Type},{Position}";
}



public enum PieceColor : byte
{
    Black,
    White,
}


public enum PieceType : byte
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}
