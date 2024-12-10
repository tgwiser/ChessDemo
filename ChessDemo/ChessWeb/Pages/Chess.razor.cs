using ChessCommon;
using ChessCommon.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;

namespace ChessWeb.Pages
{
    public partial class Chess
    {
        private int depth = 1;


        [Inject]
        protected ChessService _chessService { get; set; } 

        private Position originalPosition;

        //Evaluate move.
        private string movePosition = string.Empty;
        private int? moveCounter = null;
        private int? moveValue = null;
        private string moveTime = string.Empty;


        private string currentPlayer = "";
        private string csvFile = "";
        private string pgnStr = "";
        private string gameFilter = "";
        private string pgnFileName = "";

        private List<Move> moves = [];
        private List<string> gameList = [];
        private string Info = "";

        private bool isWhiteCheck = false;
        private bool isWhiteMate = false;

        private bool isBlackCheck = false;
        private bool isBlackMate = false;

        private bool isMate = false;
        private bool isAutoPay = true;

        private string isCurrentPlayer(PieceColor color) => chessEngine.CurrentPlayer == color ? "YES" : "NO";

        //Counter
        private void Play()
        {
            chessEngine.PlayBestMove(depth);
            moves = chessEngine.GetMoves();
        }

        private void SaveBoard() => chessEngine.SaveBoard(csvFile);

        private async Task LoadBoard()
        {
            await chessEngine.LoadBoard(csvFile);
            moves = chessEngine.GetMoves();
            Info = "File uploaded1";
        } 

        private async Task FindGames() {
            gameList = await chessEngine.FindGames(gameFilter);
        }

        private async Task DeleteGame(string name)
        {
            await chessEngine.DeleteGame(name);
            gameList = await chessEngine.FindGames();
        }

        private async Task LoadGame(string name)
        {
            try
            {
                await chessEngine.LoadBoard(name);
                moves = chessEngine.GetMoves();
            }
            catch (Exception ex)
            {
                Info = ex.Message;
            }
        }


        private void EvaluateMove()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            (movePosition, moveCounter, moveValue) = chessEngine.EvaluateBestMove(depth, chessEngine.CurrentPlayer);
            moveTime = $"{stopwatch.Elapsed.Minutes}:{stopwatch.Elapsed.Seconds}:{stopwatch.Elapsed.Milliseconds}";

            stopwatch.Stop();
        }

        protected override async Task OnInitializedAsync()
        {
            moves = chessEngine.GetMoves();
        }

        private async Task HandleDragDroped(DragEventArgs e, Position destination)
        {
            var isLegalMove = chessEngine.IsLegalMove(originalPosition, destination);

            if (isLegalMove)
            {
                chessEngine.DropPiece(originalPosition, destination);
                currentPlayer = chessEngine.CurrentPlayer.ToString();
                moves = chessEngine.GetMoves();
                if (isAutoPay && !isMate)
                {
                    await Task.Delay(1000 / depth);
                    Play();
                }
            }
        }

        private void HandleDragEnter(DragEventArgs e, Position position)
        {
            originalPosition = position;
        }


        private void LoadPgn(MouseEventArgs e)
        {
            chessEngine.LoadPgnBoard(pgnStr);
        }

        private void ResetPgnMoves(MouseEventArgs e)
        {
            chessEngine.ResetPgnMoves();
        }
    }
}