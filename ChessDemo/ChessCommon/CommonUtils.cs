using ChessCommon.Evaluators;
using ChessCommon.Evaluators.Contracts;
using ChessCommon.Models;
using System.Text;

namespace ChessCommon;

public class CommonUtils
{
    static PieceType[] pieceTypes ={
        PieceType.Rook,
        PieceType.Knight,
        PieceType.Bishop,
        PieceType.Queen,
        PieceType.King,
        PieceType.Bishop,
        PieceType.Knight,
        PieceType.Rook
    };

    /// <summary>
    /// Board y-dimension
    /// </summary>
    public const int MAX_ROWS = 8;

    /// <summary>
    /// Board x-dimension
    /// </summary>
    public const int MAX_COLS = 8;

    public static Piece?[,] GetInitChessPices()
    {
        Piece?[,] pieces = new Piece[MAX_ROWS, MAX_COLS];
        for (int i = 0; i < 8; i++)
        {
            pieces[0, i] = new Piece(PieceColor.White, pieceTypes[i], new Position(0, i));
            pieces[7, i] = new Piece(PieceColor.Black, pieceTypes[i], new Position(7, i));
        }

        for (int i = 0; i < 8; i++)
        {
            pieces[1, i] = new Piece(PieceColor.White, PieceType.Pawn, new Position(1, i));
            pieces[6, i] = new Piece(PieceColor.Black, PieceType.Pawn, new Position(6, i));
        }
        return pieces;
    }

    public static Board GetIDefaultBoard()
    {
        var board = new Board(GetInitChessPices());
        return board;
    }

    public static void SaveInfo(string fileName,bool whiteCasteleLeft, bool whiteCasteleRight, bool blackCasteleLeft, bool blackCasteleRight)
    {
        List<string> csvData =
        [
            $"WhiteCastle,{whiteCasteleLeft},{whiteCasteleRight}",
            $"blackCastle,{blackCasteleLeft},{blackCasteleRight}",
        ];
        File.WriteAllLines(fileName, csvData);
    }

    /// <summary>
    /// Short horizontal position from file char<br/>
    /// 'a' => 0<br/>
    /// 'b' => 1<br/>
    /// 'c' => 2<br/>
    /// 'd' => 3<br/>
    /// 'e' => 4<br/>
    /// 'f' => 5<br/>
    /// 'g' => 6<br/>
    /// 'h' => 7<br/>
    /// </summary>
    public static short PositionFromFile(char file)
    {
        return (short)(file - 'a');
    }

    /// <summary>
    /// Short vertical position from rank char<br/>
    /// '1' => 0<br/>
    /// '2' => 1<br/>
    /// '3' => 2<br/>
    /// '4' => 3<br/>
    /// '5' => 4<br/>
    /// '6' => 5<br/>
    /// '7' => 6<br/>
    /// '8' => 7<br/>
    /// </summary>
    public static short PositionFromRank(char rank)
    {
        return (short)(rank - '1');
    }


    public static (bool, bool) IsDestinationStateChanged(Piece piece, IBoardManager boardManager)
    {
        if (piece.Type != PieceType.King && piece.Type != PieceType.Rook)
            return (false, false);


        (bool leftCastlingEnabled, bool rightCastlingEnabled) = boardManager.GetCastleState(piece.Color);

        if (piece.Type == PieceType.King)
            return (leftCastlingEnabled, rightCastlingEnabled);

        if (piece.Type == PieceType.Rook)
            return (leftCastlingEnabled && piece.Position.X == 0, rightCastlingEnabled && piece.Position.X == 7);


        return (leftCastlingEnabled, rightCastlingEnabled);
    }


    public static List<(Position src , Position dest)> GetSrcDestData(string moveData)
    {
        var srcDest = new List<(Position src, Position dest)>();
        var moveDataArr = moveData.Split(Environment.NewLine);
       
        foreach (var item in moveDataArr)
        {
            var srcDestArr = item.Split(',');
            if(srcDestArr.Length ==2)
                srcDest.Add((new Position(srcDestArr[0]), new Position(srcDestArr[1])));
        }
        return srcDest;

    }

    internal static string GetSrcDestData(List<Move> moves)
    {
        StringBuilder movesStr = new StringBuilder();
        foreach (var move in moves)
        {
            movesStr.AppendLine(move.MoveData);
        }
        return movesStr.ToString();
    }

}