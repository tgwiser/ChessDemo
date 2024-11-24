using ChessCommon;
using ChessCommon.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;

namespace ChessWeb.Pages
{
    public partial class Chess
    {
        private int depth = 1;
        private ChessEngine chessEngine;
        private Position originalPosition;

        //Evaluate move.
        private string movePosition = string.Empty;
        private int? moveCounter = null;
        private int? moveValue = null;
        private string moveTime = string.Empty;


        private string currentPlayer = "";
        private string csvFile = "";
        private string gameFilter = "";

        private List<string> gameList = [];
        private string Info = "";

        private bool isWhiteCheck = false;
        private bool isWhiteMate = false;

        private bool isBlackCheck = false;
        private bool isBlackMate = false;

        private bool isMate = false;
        private bool isAutoPay = true;

        private string isCurrentPlayer(PieceColor color) => chessEngine.CurrentPlayer == color ? "YES" : "NO";

        //Next & Prev
        private void NextMove() => chessEngine.Next();
        private void PrevMove() => chessEngine.Prev();

        //Counter
        private void IncCount() => depth++;
        private void DecCount() => depth--;

        private void DisableAutoPlay() => isAutoPay = false;
        private void EnableAutoPlay() => isAutoPay = true;

        private void Play() => chessEngine.PlayBestMove(depth);

        private void SaveBoard() => chessEngine.SaveBoard(csvFile);

        private void LoadBoard() => Info = ChessService.LoadBoard(chessEngine, csvFile);

        private async Task FindGames() {
            gameList = await ChessService.FindGames(chessEngine, gameFilter);
        }
        private async Task DeleteGame(string name)
        {
            gameList = await ChessService.DeleteGame(chessEngine, name);
        }

        private async Task LoadGame(string name)
        {
            try
            {
                await ChessService.LoadGame(chessEngine, name);
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
            chessEngine = chessService.GetChessEngine();
        }

        private async Task HandleDragDroped(DragEventArgs e, Position destination)
        {
            var isLegalMove = chessEngine.IsLegalMove(originalPosition, destination);

            if (isLegalMove)
            {
                chessEngine.DropPiece(originalPosition, destination);
                currentPlayer = chessEngine.CurrentPlayer.ToString();

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


    }
}