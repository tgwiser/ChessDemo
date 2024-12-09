using ChessCommon.Models;
using ChessCommon.Services;
using ChessCommon.Services.Contracts;
using System.Drawing;

namespace ChessCommon;

public class ChessEngine : IChessEngine
{
    public PieceColor CurrentPlayer { get; private set; } = PieceColor.White;



    public (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) WhiteCastlingState { get { return _boardManagerService.GetCastleState(PieceColor.White); } }
    public (bool IsLeftCastlingEnabled, bool IsRightCastlingEnabled) BlackCastlingState { get { return _boardManagerService.GetCastleState(PieceColor.Black); } }

    private IPositionEvaluatorService _positionEvaluatorEngineService;
    private IGamePersistenseService _gamePersistenseManagerService;
    private IBoardManagerService _boardManagerService;
    private IGameEvaluatorService _gameEvaluatorService;
    private IPgnAnalyzerService _pgnAnalyzerService;
    private IGameHistoryService _gameHistoryService;
    private bool standardiseCastlingPositions;

    /// <summary>
    /// Creates new chess board with default pieces positions
    /// </summary>
    public ChessEngine(
        IPositionEvaluatorService positionEvaluator,
        IBoardManagerService boardManager,
        IGameEvaluatorService gameEvaluator,
        IGamePersistenseService gamePersistenseManager,
        IPgnAnalyzerService pgnAnalyzerService,
        IGameHistoryService gameHistoryService)
    {
        _positionEvaluatorEngineService = positionEvaluator;

        _boardManagerService = boardManager;
        _boardManagerService.Reset();
        _gameEvaluatorService = gameEvaluator;
        _gamePersistenseManagerService = gamePersistenseManager;
        _pgnAnalyzerService = pgnAnalyzerService;
        _gameHistoryService = gameHistoryService;
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
        _gameHistoryService.AddMove(move);

        //Reset the game evaluator.
        _gameEvaluatorService.InitPlayersPieces();

        //Pgnig.Api.Client.Models.
        //Changing turn
        CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
    }

    public List<Move> GetMoves()
    {
        return _gameHistoryService.GetMoves();
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
        var a1 = await Task.Run(() => IsLegalMove(position, destPosition));
        return a1;
    }

    public void PlayBestMove(int depth)
    {
        if (IsMate(CurrentPlayer))
            return;
        var move = _gameEvaluatorService.EvaluateBestMove(depth, CurrentPlayer);
        if (move != null)
            DropPiece(move);
    }


    public (string SelectedMove, int Counter, int BestValue) EvaluateBestMove(int depth, PieceColor color)
    {
        _gameEvaluatorService.EvaluateBestMove(depth, color);

        var selectedMove = _gameEvaluatorService?.SelectedMove?.ToString() ?? string.Empty;
        var moveCounter = _gameEvaluatorService?.Counter ?? 0;
        var moveValue = _gameEvaluatorService?.BestValue ?? 0;
        return (selectedMove, moveCounter, moveValue);
    }

    public bool IsCheck(PieceColor color)
    {
        var opnmentColor = color == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        int bestValue = EvaluateBestMove(1, opnmentColor).BestValue;
        return bestValue == 100;
    }

    public bool IsMate(PieceColor color)
    {
        int bestValue = EvaluateBestMove(2, color).BestValue;
        return bestValue < -10;
    }


    public bool TryGetNextMove()
    {
        Move? move = null;
        if (_gameHistoryService.TryGetNextMove(out move))
            DropPiece(move!);
        return move != null;
    }

    public bool TryGetPrevMove()
    {
        Move? move = null;
        if (_gameHistoryService.TryGetPrevMove(out move))
            RestorePiece(move!);
        return move != null;

    }


    //Save load games.
    public void SaveBoard(string fileName)
    {
        var moves = _gameHistoryService.GetMoves();
        var movesStr = CommonUtils.GetSrcDestData(moves);
        _gamePersistenseManagerService.SaveGame(fileName, movesStr);
    }

    public async Task LoadBoard(string fileName)
    {
        _gameEvaluatorService.InitPlayersPieces();

        _boardManagerService.Reset();

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
        //Rest history & Board.
        _boardManagerService.Reset();
        _gameHistoryService.Reset();
        
        //Load moves from pgn.
        var pgnMoves = _pgnAnalyzerService.GetPgnMovesFromPgnSrc(pgnStr);

        PieceColor currentMoveColor = PieceColor.White;

        foreach (var pgnMove in pgnMoves)
        {
            try
            {
                var move = _pgnAnalyzerService.GetMoveFromPgn(pgnMove, currentMoveColor);
                currentMoveColor = currentMoveColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
                if (move != null)
                    DropPiece(move);
            }
            catch (Exception ex) 
            { 

            }
        }

        var t1 = GetGamePgn();
    }

    public List<string> GetGamePgn()
    {
        //Rest history & Board.
        ResetPgnMoves();

        List<string> pgnMoves = new();

        string whiteMoveStr = string.Empty;
        int idx = 1;
        while (_gameHistoryService.TryGetNextMove(out Move? move))
        {
            var color = move!.Piece.Color;
            if (PieceColor.White == color)
            {
                whiteMoveStr = _pgnAnalyzerService.ConvertToPgnMove(move!, PieceColor.White);
            }
            else
            {
                var blackMoveStr = _pgnAnalyzerService.ConvertToPgnMove(move!, color);
                pgnMoves.Add($"{idx}. {whiteMoveStr} {blackMoveStr}");
                whiteMoveStr = string.Empty;
                idx++;
            }
            DropPiece(move!);
        }

        if (!string.IsNullOrEmpty(whiteMoveStr))
            pgnMoves.Add($"{whiteMoveStr}");


        File.WriteAllLines("tal.txt", pgnMoves);
        return pgnMoves;
    }



    public void ResetPgnMoves()
    {
        while (_gameHistoryService.TryGetPrevMove(out Move? move))
            _boardManagerService.RestorePiece(move);
    }

}
