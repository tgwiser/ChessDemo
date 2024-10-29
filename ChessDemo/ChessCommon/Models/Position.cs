
namespace ChessCommon.Models;

public class Position
{
    /// <summary>
    /// Horizontal position (File) on chess board
    /// </summary>
    public short X { get; internal set; } = -1;

    /// <summary>
    /// Vertical position (Rank) on chess board
    /// </summary>
    public short Y { get; internal set; } = -1;

    /// <summary>
    /// Whether X and Y are in valid range [0; ChessBoard.MAX_COLS/MAX_ROWS[
    /// </summary>
    public bool HasValue => HasValueX & HasValueY;

    /// <summary>
    /// Initializes a new Position ex.:<br/>
    /// "a1" - notation => {X = 0, Y = 0}<br/>
    /// "h8" - notation => {X = 7, Y = 7}<br/>
    /// </summary>
    /// <param name="position">Position as string</param>
    public Position(string position)
    {
        position = position.ToLower();

        X = CommonUtils.PositionFromFile(position[0]);
        Y = CommonUtils.PositionFromRank(position[1]);
    }

    /// <summary>
    /// Initializes a new Position in chess board<br/>
    /// Counting from 0 
    /// </summary>
    public Position(int y, int x)
    {
        X = (short)x;
        Y = (short)y;
    }

    /// <summary>
    /// Print Position
    /// </summary>
    public override string ToString() {
        byte[] intBytes = BitConverter.GetBytes(97 + X);
        var file = BitConverter.ToChar(intBytes).ToString();
        return file + (Y + 1);
    }



    public void SetPosition(int y, int x)
    {
        X = (short)x;
        Y = (short)y;
    }

    /// <summary>
    /// Whether X is in range [0; ChessBoard.MAX_COLS[
    /// </summary>
    private bool HasValueX => X >= 0 && X < CommonUtils.MAX_COLS;

    /// <summary>
    /// Whether Y is in range [0; ChessBoard.MAX_ROWS[
    /// </summary>
    private bool HasValueY => Y >= 0 && Y < CommonUtils.MAX_ROWS;
}
