using ChessCommon.Evaluators;
using ChessCommon.Evaluators.Contracts;
using ChessCommon.Models;

namespace ChessCommon;

public class ChessEngine
{
    public PieceColor CurrentPlayer { get; private set; } = PieceColor.White;

    GamePersistenseManager gamePersistenseManager;

    public (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) WhiteCastlingState { get { return _boardManager.GetCastleState(PieceColor.White); } }
    public (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) BlackCastlingState { get { return _boardManager.GetCastleState(PieceColor.Black); } }

    GameHistoryManager gameHistoryManager = new GameHistoryManager();
    GameEvaluator gameEvaluator;

    public IPositionEvaluator PositionEvaluatorEngine { get; }

    public IBoardManager _boardManager;

    private bool standardiseCastlingPositions;

    /// <summary>
    /// Creates new chess board with default pieces positions
    /// </summary>
    public ChessEngine(IPositionEvaluator positionEvaluator, IBoardManager boardManager)
    {
        PositionEvaluatorEngine = positionEvaluator;

        _boardManager = boardManager;
        _boardManager.Board = CommonUtils.GetIDefaultBoard();
        gameEvaluator = new GameEvaluator(PositionEvaluatorEngine, boardManager);
        gamePersistenseManager = new GamePersistenseManager(_boardManager);
    }

    public void DropPiece(Position srcPosition, Position destPosition)
    {
        // Moving piece to its new position
        Move move = new Move(
            srcPosition,
            destPosition, 
            GetPiece(srcPosition),
            GetPiece(destPosition));

        move.Piece.Position = move.DestPosition;
        DropPiece(move);
    }

    public Piece GetPiece(Position position)
    {
        return _boardManager.GetPiece(position);
    }

    internal void DropPiece(Move move)
    {
        //Update the board.
        _boardManager.DropPiece(move);

        //Update move history.
        gameHistoryManager.AddMove(move);
   
        //Reset the game evaluator.
        gameEvaluator.InitPlayersPieces();

        //Pgnig.Api.Client.Models.
        //Changing turn
        CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
    }

    internal void RestorePiece(Move move)
    {
        //Update the board.
        _boardManager.RestorePiece(move);

        //Reset the game evaluator.
        gameEvaluator.InitPlayersPieces();

        //Changing turn
        CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
    }

    public bool IsLegalMove(Position position, Position destPosition)
    {
        Piece piece = _boardManager.GetPiece(position);

        if (piece == null || piece.Color != CurrentPlayer)
            return false;

       var legalPositions = PositionEvaluatorEngine.GetLegalPositions(piece);

        var isLegalMove = legalPositions.Exists(lp => lp == destPosition);
        return isLegalMove;
    }

    public async Task<bool> IsLegalMoveAsync(Position position, Position destPosition)
    {
        var a1  = await Task.Run(() => IsLegalMove(position, destPosition));
        return a1;
    }

    public void PlayBestMove(int depth)
    {
        if (IsMate(CurrentPlayer))
            return;
        var move = gameEvaluator.EvaluateBestMove(depth, CurrentPlayer);
        if(move!=null)
            DropPiece(move);
    }


    public (string SelectedMove, int Counter , int BestValue) EvaluateBestMove(int depth, PieceColor color)
    {
        gameEvaluator.EvaluateBestMove(depth, color);

        var selectedMove = gameEvaluator?.SelectedMove?.ToString() ?? string.Empty;
        var moveCounter = gameEvaluator?.Counter ?? 0;
        var moveValue = gameEvaluator?.BestValue ?? 0;
        return (selectedMove, moveCounter, moveValue);
    }

    public bool IsCheck(PieceColor color)
    {
        var  opnmentColor = color == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        int bestValue = EvaluateBestMove(1, opnmentColor).BestValue;
        return bestValue == 100;
    }

    public bool IsMate(PieceColor color)
    {
        int bestValue = EvaluateBestMove(2, color).BestValue;
        return bestValue < -10;
    }

    public void SaveBoard(string fileName)=>  gamePersistenseManager.SaveBoard(fileName);

    public void LoadBoard(string fileName)
    {
        _boardManager.Board = gamePersistenseManager.GetBoard(fileName);
        gameEvaluator.InitPlayersPieces();
    }

    public void Next()
    {
        if (gameHistoryManager.TryGetNextMove(out Move? move))
            DropPiece(move!);
    }

    public void Prev()
    {
        if (gameHistoryManager.TryGetPrevMove(out Move? move))
            RestorePiece(move!);
    }
}
