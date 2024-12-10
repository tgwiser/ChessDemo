using ChessCommon.Models;
using ChessCommon.Services;
using ChessCommon.Services.Contracts;
using System.Drawing;
using System.Text;

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
        var movesStr = GetGamePgn();
        _gamePersistenseManagerService.SaveGame(fileName, movesStr);
    }

    public async Task LoadBoard(string fileName)
    {
        //Rest history & Board.
        _gameEvaluatorService.InitPlayersPieces();
        _boardManagerService.Reset();
        _gameHistoryService.Reset();

        var game = await _gamePersistenseManagerService.GetGame(fileName);
        LoadPgnBoard(game.Moves);
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
        //Load moves from pgn.
        var pgnMoves = _pgnAnalyzerService.GetPgnMovesFromPgnSrc(pgnStr);

        PieceColor currentMoveColor = PieceColor.White;

        int idx = 0;
        foreach (var pgnMove in pgnMoves)
        {
            idx++;

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
    }

    public string GetGamePgn()
    {
        //Rest history & Board.
        ResetPgnMoves();

        StringBuilder pgnMoves = new();

        string whiteMoveStr = string.Empty;
        string blackMoveStr = string.Empty;
        int idx = 1;
        while (_gameHistoryService.TryGetNextMove(out Move? move))
        {
            if (PieceColor.White == move!.Piece.Color)
            {
                whiteMoveStr = _pgnAnalyzerService.ConvertToPgnMove(move!, PieceColor.White);
            }
            else
            {
                blackMoveStr = _pgnAnalyzerService.ConvertToPgnMove(move!, PieceColor.Black);
                pgnMoves.AppendLine($"{idx++}. {whiteMoveStr} {blackMoveStr}");
                whiteMoveStr = string.Empty;
            }
            DropPiece(move!);
        }

        if (!string.IsNullOrEmpty(whiteMoveStr))
            pgnMoves.AppendLine($"{idx++}. {whiteMoveStr}");

        return pgnMoves.ToString();
    }



    public void ResetPgnMoves()
    {
        while (_gameHistoryService.TryGetPrevMove(out Move? move))
            _boardManagerService.RestorePiece(move);
    }

    public void ExportPgn(string fileName)
    {
        string data = GetGamePgn();
        File.WriteAllText(fileName + ".txt", data);
    }

}
