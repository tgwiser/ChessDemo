using ChessCommon.Services;
using ChessCommon.Services.Contracts;
using ChessCommon.Models;

namespace ChessCommon;

public class ChessEngine : IChessEngine
{
    public PieceColor CurrentPlayer { get; private set; } = PieceColor.White;

    

    public (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) WhiteCastlingState { get { return _boardManagerService.GetCastleState(PieceColor.White); } }
    public (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) BlackCastlingState { get { return _boardManagerService.GetCastleState(PieceColor.Black); } }

    GameHistoryService gameHistoryManager = new GameHistoryService();

    private IPositionEvaluatorService _positionEvaluatorEngineService;
    private IGamePersistenseService _gamePersistenseManagerService;
    private IBoardManagerService _boardManagerService;
    private IGameEvaluatorService _gameEvaluatorService;
    private IPgnAnalyzerService _pgnAnalyzerService;

    private bool standardiseCastlingPositions;

    /// <summary>
    /// Creates new chess board with default pieces positions
    /// </summary>
    public ChessEngine(
        IPositionEvaluatorService positionEvaluator, 
        IBoardManagerService boardManager, 
        IGameEvaluatorService gameEvaluator, 
        IGamePersistenseService gamePersistenseManager,
        IPgnAnalyzerService pgnAnalyzerService)
    {
        _positionEvaluatorEngineService = positionEvaluator;

        _boardManagerService = boardManager;
        _boardManagerService.Board = CommonUtils.GetIDefaultBoard();
        _gameEvaluatorService = gameEvaluator;
        _gamePersistenseManagerService = gamePersistenseManager;
        _pgnAnalyzerService = pgnAnalyzerService;
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
        return _boardManagerService.GetPiece(position);
    }

    internal void DropPiece(Move move)
    {
        //Update the board.
        _boardManagerService.DropPiece(move);

        //Update move history.
        gameHistoryManager.AddMove(move);
   
        //Reset the game evaluator.
        _gameEvaluatorService.InitPlayersPieces();

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
        _boardManagerService.RestorePiece(move);

        //Reset the game evaluator.
        _gameEvaluatorService.InitPlayersPieces();

        //Changing turn
        CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
    }

    public bool IsLegalMove(Position position, Position destPosition)
    {
        Piece piece = _boardManagerService.GetPiece(position);

        if (piece == null || piece.Color != CurrentPlayer)
            return false;

       var legalPositions = _positionEvaluatorEngineService.GetLegalPositions(piece);

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
        var move = _gameEvaluatorService.EvaluateBestMove(depth, CurrentPlayer);
        if(move!=null)
            DropPiece(move);
    }


    public (string SelectedMove, int Counter , int BestValue) EvaluateBestMove(int depth, PieceColor color)
    {
        _gameEvaluatorService.EvaluateBestMove(depth, color);

        var selectedMove = _gameEvaluatorService?.SelectedMove?.ToString() ?? string.Empty;
        var moveCounter = _gameEvaluatorService?.Counter ?? 0;
        var moveValue = _gameEvaluatorService?.BestValue ?? 0;
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
        _gamePersistenseManagerService.SaveGame(fileName, movesStr);
    }

    public async Task LoadBoard(string fileName)
    {
        _gameEvaluatorService.InitPlayersPieces();
        _boardManagerService.Board = CommonUtils.GetIDefaultBoard();

        var game = await _gamePersistenseManagerService.GetGame(fileName);
        var moveData = CommonUtils.GetSrcDestData(game.Moves);
        foreach (var srcDest in moveData)
        {
            DropPiece(srcDest.src, srcDest.dest);
        }
    }

    public async Task<List<string>> FindGames()
    {
        var games = await _gamePersistenseManagerService.GetGameNames(string.Empty);
        return games;
    }

    public async Task<List<string>> FindGames(string filter)
    {
        var games = await _gamePersistenseManagerService.GetGameNames(filter);
        return games;
    }

    public async Task DeleteGame(string name)
    {
        await _gamePersistenseManagerService.DeleteGame(name);
    }


    //Pgn.
    public void LoadPgnBoard(string pgnStr)
    {
         var pgnMoves = _pgnAnalyzerService.LoadGame(pgnStr);

        foreach (var pgnMove in pgnMoves)
        {
            DropPiece(pgnMove.WhiteMove);

            if (pgnMove.BlackMove != null)
                DropPiece(pgnMove.BlackMove);
        }
    }



    public void ResetPgnMoves()
    {
        while (gameHistoryManager.TryGetPrevMove(out Move? move))
            _boardManagerService.RestorePiece(move);
    }

}
