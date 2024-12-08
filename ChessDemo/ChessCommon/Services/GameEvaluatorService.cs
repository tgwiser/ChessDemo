using ChessCommon.Models;
using ChessCommon.Services.Contracts;

namespace ChessCommon.Services;

internal class GameEvaluatorService : IGameEvaluatorService
{
    public int maxDepth = 3;

    IPositionEvaluatorService _positionEvaluator;
    IBoardManagerService _boardManager;
    private IDictionary<int, Piece> BlackPieces = new Dictionary<int, Piece>();
    private IDictionary<int, Piece> WhitePieces = new Dictionary<int, Piece>();
    PieceColor CurrentPlayer;

    public Move? SelectedMove { get; private set; }

    public int Counter { get; private set; }
    public int BestValue { get; private set; }

    public GameEvaluatorService(IPositionEvaluatorService positionEvaluator, IBoardManagerService boardManager)
    {
        _positionEvaluator = positionEvaluator;
        _boardManager = boardManager;
    }

    public void InitPlayersPieces()
    {
        BlackPieces = new Dictionary<int, Piece>();
        WhitePieces = new Dictionary<int, Piece>();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = _boardManager.GetPiece(i, j);
                if (piece != null)
                {
                    var key = GetPieceHashKey(piece.Position);
                    if (piece.Color == PieceColor.White)
                        WhitePieces.Add(key, piece);
                    else
                        BlackPieces.Add(key, piece);
                }
            }
        }
    }

    private int EvaluateGame(Board board)
    {
        int totalWhite = 0;
        int totalBlack = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = _boardManager.GetPiece(y, x);
                if (piece == null)
                    continue;

                if (piece.Color == PieceColor.White)
                    totalWhite = EvaluatePiece(piece);
                else
                    totalBlack = EvaluatePiece(piece);
            }
        }
        return totalWhite - totalBlack;
    }

    public Move? EvaluateBestMove(int depth, PieceColor selectedPlayer)
    {
        Counter = 0;
        BestValue = -1;
        maxDepth = depth;
        CurrentPlayer = selectedPlayer;

        EvaluateBestMove(depth, selectedPlayer, true);
        return SelectedMove;
    }


    private int EvaluateBestMove(int depth, PieceColor selectedPlayer, bool isMax)
    {
        var playerPieces = selectedPlayer == PieceColor.White ? WhitePieces : BlackPieces;
        var currentLevelBestValue = isMax ? -1000 : 1000;
        Random rnd = new Random();
        foreach (Piece piece in playerPieces.Values.ToList())
        {
            var legalPositions = _positionEvaluator.GetLegalPositions(piece);

            foreach (Position destPosition in legalPositions)
            {
                Counter++;
                int capturePieceValue = EvaluatePiece(piece, destPosition, isMax);

                Move move = new Move(piece.Position, destPosition, piece, _boardManager.GetPiece(destPosition));

                if (depth == 1)
                {
                    if (IsBestValue(currentLevelBestValue, capturePieceValue, isMax))
                    {
                        currentLevelBestValue = capturePieceValue;
                        if (depth == maxDepth)
                        {
                            SelectedMove = new Move(piece.Position, destPosition, _boardManager.GetPiece(piece.Position), _boardManager.GetPiece(destPosition));
                            BestValue = currentLevelBestValue;
                        }
                    }
                }
                else
                {
                    //Create move
                    var srcKey = GetPieceHashKey(move.SrcPosition);
                    var destKey = GetPieceHashKey(move.DestPosition);

                    DropPiece(move, srcKey, destKey, selectedPlayer);

                    //bool isSkipprd =   rnd.Next(1, 20)  < 8;

                    //if (isSkipprd)
                    //    capturePieceValue = isMax ? 10 : -10;
                    //else
                    capturePieceValue = EvaluateBestMove(depth - 1, CurrentPlayer, !isMax) + capturePieceValue;

                    if (IsBestValue(currentLevelBestValue, capturePieceValue, isMax))
                    {
                        currentLevelBestValue = capturePieceValue;
                        if (depth == maxDepth)
                        {
                            SelectedMove = move;
                            BestValue = currentLevelBestValue;

                        }
                    }

                    RestorePiece(move, srcKey, destKey, selectedPlayer);
                }
            }


        }
        return currentLevelBestValue;
    }

    private void RestorePiece(Move move, int playerKey, int destinationKey, PieceColor color)
    {
        var currentPieces = color == PieceColor.White ? WhitePieces : BlackPieces;
        var oponmentPieces = color == PieceColor.White ? BlackPieces : WhitePieces;

        // Moving piece to its new position
        if (move.CapturedPiece != null)
            oponmentPieces.Add(destinationKey, move.CapturedPiece);

        currentPieces.Remove(destinationKey);
        currentPieces.Add(playerKey, move.Piece);

        if (move.Castle != null)
        {
            Piece theRock = null;
            try
            {
                theRock = currentPieces[move.Castle.DestRockKey];
                currentPieces.Remove(move.Castle.DestRockKey);
                currentPieces.Add(move.Castle.SrcRockKey, theRock);
            }
            catch (Exception)
            {

            }

        }

        //Update board
        _boardManager.RestorePiece(move);

        //Changing turn
        CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
    }

    public void DropPiece(Move move, int playerKey, int destinationKey, PieceColor color)
    {
        //Update board
        _boardManager.DropPiece(move);

        var currentPieces = color == PieceColor.White ? WhitePieces : BlackPieces;
        var oponmentPieces = color == PieceColor.White ? BlackPieces : WhitePieces;

        // Moving piece to its new position
        if (move.CapturedPiece != null)
            oponmentPieces.Remove(destinationKey);

        currentPieces.Remove(playerKey);
        currentPieces.Add(destinationKey, move.Piece);

        if (move.Castle != null)
        {
            var theRock = currentPieces[move.Castle.SrcRockKey];
            currentPieces.Remove(move.Castle.SrcRockKey);
            currentPieces.Add(move.Castle.DestRockKey, theRock);
        }

        //Changing turn
        CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
    }


    private bool IsBestValue(int bestValue, int captureValue, bool isMax)
    {
        return (isMax && bestValue < captureValue || !isMax && captureValue < bestValue);
    }

    private int EvaluatePiece(Piece piece, Position position, bool isMax)
    {
        int tempValue = EvaluatePiece(_boardManager.GetPiece(position));
        if (piece.Type == PieceType.Pawn && (position.Y == 0 || position.Y == 7))
            tempValue += 9;
        return isMax ? tempValue : tempValue * -1;
    }

    private int EvaluatePiece(Piece piece)
    {
        if (piece == null)
            return 0;

        return piece.Type switch
        {
            PieceType.Pawn => 1,
            PieceType.King => 100,
            PieceType.Rook => 5,
            PieceType.Bishop => 3,
            PieceType.Knight => 3,
            PieceType.Queen => 9,
            _ => -1,
        };
    }

    private int GetPieceHashKey(Position p) => p.X + p.Y * 8;

}
