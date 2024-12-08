
namespace ChessCommon.Models;

public class Castle
{
    public bool IsCastle { get; private set; }
    public Position DestRock { get; internal set; }
    public Position SrcRock { get; internal set; }

    public int SrcRockKey { get { return SrcRock.Y * 8 + SrcRock.X; } }

    public int DestRockKey { get { return DestRock.Y * 8 + DestRock.X; } }

    public bool IsRightCastle;
    public bool IsLeftCastle;

    public Castle()
    {
    }

    public Castle(Position originalPosition)
    {
        IsRightCastle = originalPosition.X == 6;
        IsLeftCastle = originalPosition.X == 2;
        SrcRock = new Position(originalPosition.Y, IsRightCastle ? 7 : 0);
        DestRock = new Position(originalPosition.Y, IsRightCastle ? 5 : 3);
    }

}
