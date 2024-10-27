using ChessCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCommon.Evaluators
{
    public class GameEvaluator : IGameEvaluator
    {
        public int maxDepth = 3;

        IPositionEvaluator _positionEvaluator;
        Board _board;
        private IDictionary<int, Piece> BlackPieces = new Dictionary<int, Piece>();
        private IDictionary<int, Piece> WhitePieces = new Dictionary<int, Piece>();
        PieceColor CurrentPlayer;

        public Move? SelectedMove { get; private set; }

        public int Counter { get; private set; }
        public int BestValue { get; private set; }
     
        public GameEvaluator(IPositionEvaluator positionEvaluator, Board board)
        {
            _positionEvaluator = positionEvaluator;
            _board = board;
            InitPlayersPieces(board);
        }

    

        private void InitPlayersPieces(Board board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = board.Pieces[i, j]!;
                    if (piece != null)
                    {
                        var key = GetPieceHashKey(piece.Position);
                        var pieces = piece.Color == PieceColor.White ? WhitePieces : BlackPieces;
                        pieces.Add(key, piece);
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
                    var piece = board.Pieces[x, y]!;

                    if (piece != null && piece.Color == PieceColor.White)
                        totalWhite = EvaluatePiece(piece);
                    if (piece != null && piece.Color == PieceColor.Black)
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
            var playerPieces = selectedPlayer == PieceColor.White ? WhitePieces : BlackPieces;

            if (depth == 1)
            {
                foreach (Piece piece in playerPieces.Values.ToList())
                {
                    List<Destination> legalPositions = _positionEvaluator.GetLegalPositions(piece, _board);



                    foreach (Destination destPosition in legalPositions)
                    {
                        Counter++;
                        int capturePieceValue = EvaluatePiece(piece, destPosition, true);
                        if (IsBestValue(BestValue, capturePieceValue, true))
                        {
                            //Create move
                            Move move = new Move(piece.Position, destPosition, _board[piece.Position]!, _board[destPosition]!);
                            SelectedMove = move;
                            BestValue = capturePieceValue;
                        }
                    }
                }
                return SelectedMove;
            }
            else
            {
                EvaluateBestMove(depth, selectedPlayer, true);
                return SelectedMove;
            }

        }



        private int EvaluateBestMove(int depth, PieceColor selectedPlayer, bool isMax)
        {
            var playerPieces = selectedPlayer == PieceColor.White ? WhitePieces : BlackPieces;
            var currentLevelBestValue = isMax ? -1000 : 1000;
            foreach (Piece piece in playerPieces.Values.ToList())
            {
                List<Destination> legalPositions = _positionEvaluator.GetLegalPositions(piece, _board);

                foreach (Destination destPosition in legalPositions)
                {
                    Counter++;
                    int capturePieceValue = EvaluatePiece(piece, destPosition, isMax);

                    if (depth == 1)
                    {
                        if (IsBestValue(currentLevelBestValue, capturePieceValue, isMax))
                        {
                            currentLevelBestValue = capturePieceValue;
                        }
                    }
                    else
                    {
                        //Create move
                        Move move = new Move(piece.Position, destPosition, _board[piece.Position]!, _board[destPosition]!);
                        var playerKey = GetPieceHashKey(move.SrcPosition);
                        var destinationKey = GetPieceHashKey(move.DestPosition);

                        DropPiece(move, playerKey, destinationKey, selectedPlayer);

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

                        RestorePiece(move, playerKey, destinationKey, selectedPlayer);
                    }
                }


            }
            return currentLevelBestValue;
        }

        private void RestorePiece(Move move, int playerKey, int destinationKey, PieceColor color)
        {
            var currentPieces = color == PieceColor.White ? WhitePieces : BlackPieces;
            var oponmentPieces = color == PieceColor.White ? BlackPieces : WhitePieces;

            if (move.CapturedPiece != null)
            {
                oponmentPieces.Add(destinationKey, move.CapturedPiece);
            }

            if (!currentPieces.ContainsKey(destinationKey))
            {

            }
            if (currentPieces.ContainsKey(playerKey))
            {

            }

            currentPieces.Remove(destinationKey);
            currentPieces.Add(playerKey, move.Piece);

            if (move.IsCastle)
            {
                var theRock = currentPieces[move.Castle.DestRockKey];
                currentPieces.Remove(move.Castle.DestRockKey);
                currentPieces.Add(move.Castle.SrcRockKey, theRock);
            }

            //Changing turn
            CurrentPlayer = CurrentPlayer == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        }

        private void DropPiece(Move move, int playerKey, int destinationKey, PieceColor color)
        {
            var currentPieces = color == PieceColor.White ? WhitePieces : BlackPieces;
            var oponmentPieces = color == PieceColor.White ? BlackPieces : WhitePieces;


            // Moving piece to its new position
            if (move.CapturedPiece != null)
                oponmentPieces.Remove(destinationKey);

            currentPieces.Remove(playerKey);
            currentPieces.Add(destinationKey, move.Piece);

            if (move.IsCastle)
            {
                var theRock = currentPieces[move.Castle.SrcRockKey];
                currentPieces.Remove(move.Castle.SrcRockKey);
                currentPieces.Add(move.Castle.DestRockKey, theRock);
                //theRock.Position = move.Castle.SrcRock;
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
            int tempValue = EvaluatePiece(_board.Pieces[position.Y, position.X]!);
            if (piece.Type == PieceType.Pawn && (position.Y == 0 || position.Y == 7))
                tempValue += 9;
            return isMax ? tempValue : tempValue * -1;
        }

        private int EvaluatePiece(Piece piece)
        {
            if (piece == null)
                return 0;

            switch (piece.Type)
            {
                case PieceType.Pawn:
                    return 1;
                case PieceType.King:
                    return 100;
                case PieceType.Rook:
                    return 5;
                case PieceType.Bishop:
                    return 3;
                case PieceType.Knight:
                    return 3;
                case PieceType.Queen:
                    return 9;
            }

            return -1;
        }

        private int GetPieceHashKey(Position p) => p.X + p.Y * 8;

    }
}
