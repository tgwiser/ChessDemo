using ChessCommon.Evaluators.Contracts;
using ChessCommon.Models;

namespace ChessCommon.Evaluators;

internal class GamePersistenseManager
{
    /// <summary>
    /// Board y-dimension
    /// </summary>
    public const int MAX_ROWS = 8;

    /// <summary>
    /// Board x-dimension
    /// </summary>
    public const int MAX_COLS = 8;

    public IBoardManager _boardManager;
    public GamePersistenseManager(IBoardManager boardManager) {
        _boardManager = boardManager;
    }

    public void SaveBoard(string fileName)
    {
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            string folderName = $"Data/{fileName}/";
            Directory.CreateDirectory(folderName);
            SaveBoard(folderName + "pieces.csv", _boardManager.GetPieces());

            var bord = _boardManager!.Board!;

            var whiteCastleState = _boardManager.GetCastleState(PieceColor.White);
            var blackCastleState = _boardManager.GetCastleState(PieceColor.Black);
          
            CommonUtils.SaveInfo(folderName + "info.csv",
               whiteCastleState.IsLeftCastlingEnabled,
               whiteCastleState.IsRightCastlingEnabled,
               blackCastleState.IsLeftCastlingEnabled,
               blackCastleState.IsRightCastlingEnabled);
        }
    }


    public static void SaveBoard(string fileName, Piece?[,] pieces)
    {
        List<string> csvData = new List<string>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece? piece = pieces[x, y];
                if (piece != null)
                    csvData.Add(piece.ToString());
            }
        }

        File.WriteAllLines(fileName, csvData);
    }

    public Board GetBoard(string fileName)
    {
        string piecesFileName = $"Data/{fileName}/pieces.csv";
        var pieces = GetBoardPieces(fileName);

        string infoFileName = $"Data/{fileName}/info.csv";
        (bool IsWhiteLeftCastlingEnabled, bool IsWhiteRightCastlingEnabled, bool IsBlackLeftCastlingEnabled, bool IsBlackRightCastlingEnabled)  = GetBoardInfo(infoFileName);

        Board board = new Board(pieces);
        board.InitState(IsWhiteLeftCastlingEnabled, IsWhiteRightCastlingEnabled, IsBlackLeftCastlingEnabled, IsBlackRightCastlingEnabled);

        return board;
    }


    public static Piece?[,] GetBoardPieces(string fileName)
    {
        var csvData = File.ReadAllLines(fileName);
        var pieces = new Piece[MAX_ROWS, MAX_COLS];
        foreach (var data in csvData)
        {
            var rawData = data.Split(',');


            Position position = new Position(rawData[2]);

            if (Enum.TryParse(rawData[0], out PieceColor color) &&
                Enum.TryParse(rawData[1], out PieceType pieceType))
            {
                pieces[position.Y, position.X] = new Piece(color, pieceType, position);
            }

        }
        return pieces;
    }

    public static (bool IsWhiteLeftCastlingEnabled, bool IsWhiteRightCastlingEnabled, bool IsBlackLeftCastlingEnabled, bool IsBlackRightCastlingEnabled) GetBoardInfo(string fileName)
    {
        var csvData = File.ReadAllLines(fileName);

        (bool isWhiteRightCastlingEnabled, bool isWhiteLeftCastlingEnabled) = IsEnabledCastle(csvData[0]);
        (bool isBlackLeftCastlingEnabled, bool isBlackRightCastlingEnabled) = IsEnabledCastle(csvData[1]);

        return (isWhiteLeftCastlingEnabled, isWhiteRightCastlingEnabled, isBlackLeftCastlingEnabled, isBlackRightCastlingEnabled);
    }

    public static (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) IsEnabledCastle(string info)
    {
        var infoArr = info.Split(',');
        bool isLeftCastlingEnabled = infoArr[1] == "true";
        bool isRightCastlingEnabled = infoArr[2] == "true";

        return (isLeftCastlingEnabled, isRightCastlingEnabled);
    }
}
