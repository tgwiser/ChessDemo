using ChessCommon.Services;
using ChessCommon.Services.Contracts;
using ChessCommon.Models;

namespace ChessCommon;

public class ChessEngine : IChessEngine
{
    public PieceColor CurrentPlayer { get; private set; } = PieceColor.White;

    IGamePersistenseService _gamePersistenseManager;

    public (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) WhiteCastlingState { get { return _boardManager.GetCastleState(PieceColor.White); } }
    public (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) BlackCastlingState { get { return _boardManager.GetCastleState(PieceColor.Black); } }

    GameHistoryService gameHistoryManager = new GameHistoryService();

    public IPositionEvaluatorService PositionEvaluatorEngine { get; }

    public IBoardManagerService _boardManager;
    public IGameEvaluatorService _gameEvaluator;

    private bool standardiseCastlingPositions;

    /// <summary>
    /// Creates new chess board with default pieces positions
    /// </summary>
    public ChessEngine(IPositionEvaluatorService positionEvaluator, IBoardManagerService boardManager, IGameEvaluatorService gameEvaluator, IGamePersistenseService gamePersistenseManager)
    {
        PositionEvaluatorEngine = positionEvaluator;

        _boardManager = boardManager;
        _boardManager.Board = CommonUtils.GetIDefaultBoard();
        _gameEvaluator = gameEvaluator;
        _gamePersistenseManager = gamePersistenseManager;
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
        _gameEvaluator.InitPlayersPieces();

        //Pgnig.Api.Client.Models.
        //Changing turn
        CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
    }

    public List<Move> GetMoves()
    {
        return gameHistoryManager.GetMoves();
    }


    internal void RestorePiece(Move move)
    {
        //Update the board.
        _boardManager.RestorePiece(move);

        //Reset the game evaluator.
        _gameEvaluator.InitPlayersPieces();

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
        var move = _gameEvaluator.EvaluateBestMove(depth, CurrentPlayer);
        if(move!=null)
            DropPiece(move);
    }


    public (string SelectedMove, int Counter , int BestValue) EvaluateBestMove(int depth, PieceColor color)
    {
        _gameEvaluator.EvaluateBestMove(depth, color);

        var selectedMove = _gameEvaluator?.SelectedMove?.ToString() ?? string.Empty;
        var moveCounter = _gameEvaluator?.Counter ?? 0;
        var moveValue = _gameEvaluator?.BestValue ?? 0;
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


    //Save load games.
    public void SaveBoard(string fileName)
    {
        var moves = gameHistoryManager.GetMoves();
        var movesStr = CommonUtils.GetSrcDestData(moves);
        _gamePersistenseManager.SaveGame(fileName, movesStr);
    }

    public async Task LoadBoard(string fileName)
    {
        _gameEvaluator.InitPlayersPieces();
        _boardManager.Board = CommonUtils.GetIDefaultBoard();

        var game = await _gamePersistenseManager.GetGame(fileName);
        var moveData = CommonUtils.GetSrcDestData(game.Moves);
        foreach (var srcDest in moveData)
        {
            DropPiece(srcDest.src, srcDest.dest);
        }
    }

    public async Task<List<string>> FindGames()
    {
        var games = await _gamePersistenseManager.GetGameNames(string.Empty);
        return games;
    }

    public async Task<List<string>> FindGames(string filter)
    {
        var games = await _gamePersistenseManager.GetGameNames(filter);
        return games;
    }

    public async Task DeleteGame(string name)
    {
        await _gamePersistenseManager.DeleteGame(name);
    }

 
}
